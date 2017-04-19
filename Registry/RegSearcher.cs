using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Win32;
using System.Threading;
using CrackSoft.RegExplore.Comparers;

namespace CrackSoft.RegExplore.Registry
{
    enum RegSearchLookAt
	{
        Keys = 1,
        Values = 2,
        Data = 4
	} 

    class RegSearchArgs
    {
        RegSearchLookAt lookAt;
        bool lookAtKeys;
        bool lookAtValues;
        bool lookAtData;
        bool lookAtValuesOrData;

        public bool LookAtKeys
        { 
            get { return lookAtKeys; } 
        }
        
        public bool LookAtValues 
        { 
            get { return lookAtValues; } 
        }
        
        public bool LookAtData 
        { 
            get { return lookAtData; } 
        }
        
        public bool LookAtValuesOrData 
        { 
            get { return lookAtValuesOrData; } 
        }

        public bool MatchCase { get; set; }        

        public RegSearchLookAt LookAt 
        { 
            get
            {
                return lookAt;
            }
            set
            {
                lookAt = value;
                lookAtKeys = (lookAt & RegSearchLookAt.Keys) == RegSearchLookAt.Keys;
                lookAtValues = (lookAt & RegSearchLookAt.Values) == RegSearchLookAt.Values;
                lookAtData = (lookAt & RegSearchLookAt.Data) == RegSearchLookAt.Data;
                lookAtValuesOrData = lookAtValues || lookAtData;                
            }
        }
        
        public RegistryKey[] RootKeys { get; set; }
        
        public string Pattern { get; set; }
        
        public bool UseRegEx { get; set; }

        public RegSearchArgs(RegistryKey[] regKeys, string pattern, bool matchCase, RegSearchLookAt lookAt, bool useRegEx)
        {
            RootKeys = regKeys;
            Pattern = pattern;
            MatchCase = matchCase;
            LookAt = lookAt;
            UseRegEx = useRegEx;
        }
    }

    class RegSearchMatch
    {
        public string Key { get; private set; }
        public string Value { get; private set; }
        public string Data { get; private set; }

        public RegSearchMatch(string key, string value, string data)
        {
            Key = key;
            Value = value;
            Data = data;
        }

        public override string ToString()
        {
            return String.Format("({0}:{1}:{2})", Key, Value, Data);
        }
    }

    class MatchFoundEventArgs : EventArgs
    {
        public RegSearchMatch Match { get; private set; }
        
        public MatchFoundEventArgs(RegSearchMatch match)
        {
            Match = match;
        }
    }

    class SearchCompleteEventArgs : EventArgs
    {
        public List<RegSearchMatch> Matches { get; private set; }

        public SearchCompleteEventArgs(List<RegSearchMatch> matches)
        {
            Matches = matches;
        }
    }

    class RegSearcher
    {
        BackgroundWorker worker;
        public event EventHandler<SearchCompleteEventArgs> SearchComplete;
        public event EventHandler<MatchFoundEventArgs> MatchFound;
        RegSearchArgs searchArgs;        
        List<RegSearchMatch> matches;
        Queue<string> pendingKeys;
        CrackSoft.RegExplore.Comparers.Comparer comparer;

        public RegSearcher()
        {
            worker = new BackgroundWorker() {WorkerSupportsCancellation = true, WorkerReportsProgress = true};
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
        }

        public void Start(RegSearchArgs args)
        {
            searchArgs = args;
            if (args.UseRegEx)
                comparer = new RegexComparer(args.Pattern, !args.MatchCase);
            else
                comparer = new RegExplore.Comparers.StringComparer(args.Pattern, !args.MatchCase);

            matches = new List<RegSearchMatch>();
            worker.RunWorkerAsync();
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MatchFound(this, new MatchFoundEventArgs((RegSearchMatch)e.UserState));
        }

        public void Stop()
        {
            if (worker.IsBusy)
            {
                lock (worker)
                {
                    worker.CancelAsync();
                    Monitor.Wait(worker); 
                }
            }
        }

        public bool IsBusy 
        {
            get { return worker.IsBusy; }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SearchComplete(this, new SearchCompleteEventArgs(matches));
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {            
            foreach (RegistryKey key in searchArgs.RootKeys)
                Search(key);
        }

        void Search(RegistryKey rootKey)
        {
            string rootKeyName = rootKey.Name.Substring(rootKey.Name.LastIndexOf('\\') + 1);
            ProcessKey(rootKey, rootKeyName);
            
            RegistryKey subKey = null;
            string keyName;
            string parentPath;
            int cropIndex = rootKey.Name.Length + 1;
            pendingKeys = new Queue<string>(rootKey.GetSubKeyNames());

            while (pendingKeys.Count > 0)
            {
                if (worker.CancellationPending)
                {
                    lock (worker)
                    {
                        Monitor.Pulse(worker); // for synchronous Stop()
                        return;
                    }                    
                }
                keyName = pendingKeys.Dequeue();

                try
                {
                    subKey = rootKey.OpenSubKey(keyName);
                }
                catch (System.Security.SecurityException)
                {
                    subKey = null;
                }

                if (subKey != null)
                {
                    ProcessKey(subKey, keyName);
                    parentPath = subKey.Name.Substring(cropIndex) + '\\';
                    EnqueueSubKeys(subKey, parentPath);
                }
            }
        }

        private void EnqueueSubKeys(RegistryKey key, string parentPath)
        {
            foreach (string name in key.GetSubKeyNames())
                pendingKeys.Enqueue(String.Concat(parentPath, name));
        }

        private void ProcessKey(RegistryKey key, string keyName)
        {
            if (searchArgs.LookAtKeys)
                MatchKey(key, keyName);
            if (searchArgs.LookAtValuesOrData)
            {
                foreach (string valueName in key.GetValueNames())
                {
                    if (worker.CancellationPending) return;
                    if (searchArgs.LookAtValues)
                        MatchValue(key, valueName);
                    if (searchArgs.LookAtData)
                        MatchData(key, valueName);
                }
            }
        }

        private void MatchData(RegistryKey key, string valueName)
        {
            string valueData;
            valueData = RegValue.ToString(key.GetValue(valueName, String.Empty));
            if (comparer.IsMatch(valueData))
                AddMatch(key.Name, valueName, valueData);
        }

        private void MatchValue(RegistryKey key, string valueName)
        {
            if (comparer.IsMatch(valueName))
                    AddMatch(key.Name, valueName, "-");
        }

        private void MatchKey(RegistryKey key, string keyName)
        {
            if (comparer.IsMatch(keyName))
                AddMatch(key.Name, "-", "-");
        }

        private void AddMatch(string key, string value, string data)
        {
            RegSearchMatch match = new RegSearchMatch(key, value, data);
            if (MatchFound != null)
                worker.ReportProgress(0, match);
            else if (SearchComplete != null)
                matches.Add(match);
        }
    }
}
