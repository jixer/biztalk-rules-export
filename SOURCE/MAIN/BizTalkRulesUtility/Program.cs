using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.BizTalk.RuleEngineExtensions;
using Microsoft.RuleEngine;

namespace BizTalkRulesUtility
{
    class Program
    {
        private const string vocabExportPath = "vocab.export.xml";
        private const string policyExportPath = "policy.export.xml";

        static void Main(string[] args)
        {
            if (args != null && args.Length > 0 && args[0] == "iv")
            {
                ImportVocabularies();
            }
            else if (args != null && args.Length > 0 && args[0] == "ip")
            {
                ImportPolicies();
            }
            else
            {
                ExportVocabularies();
                ExportPolicies();
            }            
        }

        private static void ImportPolicies()
        {
            // RuleSetDeploymentDriver has the following important methods            
            Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver dd = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver();
           
            // SqlRuleStore - gives access t0 the rule engine database
            SqlRuleStore sqlRuleStore = (SqlRuleStore)dd.GetRuleStore();

            // Establish the export file
            FileRuleStore fileRuleStore = new FileRuleStore(policyExportPath);

            CopyPolicies(fileRuleStore, sqlRuleStore, dd);
        }

        private static void ExportPolicies()
        {
            // RuleSetDeploymentDriver has the following important methods            
            Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver dd = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver();

            // SqlRuleStore - gives access t0 the rule engine database
            SqlRuleStore sqlRuleStore = (SqlRuleStore)dd.GetRuleStore();

            // Establish the export file
            FileRuleStore fileRuleStore = new FileRuleStore(policyExportPath);

            CopyPolicies(sqlRuleStore, fileRuleStore, dd);
        }

        //Importing policies
        private static void CopyPolicies(RuleStore sourceRuleStore, RuleStore targetRuleStore, Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver dd)
        {
            RuleSetInfoCollection sourceRulesetInfoList = sourceRuleStore.GetRuleSets(RuleStore.Filter.All);
            RuleSetInfoCollection targetRulesetInfoList = targetRuleStore.GetRuleSets(RuleStore.Filter.All);

            foreach (RuleSetInfo targetItem in targetRulesetInfoList)
            {
                if (targetItem.Published) { 
                
                } 

                
            }
   
            foreach (RuleSetInfo item in sourceRulesetInfoList)
            {
                RuleSet policy = sourceRuleStore.GetRuleSet(item);

                RuleSet targetPolicy = targetRuleStore.GetRuleSet(item);


                try
                {
                    System.Console.Out.WriteLine("Importing Policy ({0}) .." , policy.Name);
                    targetRuleStore.Add(policy);
                }
                catch (Microsoft.RuleEngine.RuleStoreRuleSetAlreadyPublishedException e)
                {
                    System.Console.Out.WriteLine("Importing Policy ({0}) : (RuleStoreRuleSetAlreadyPublishedException) Undeploying RulesetInfo {1}", policy.Name, item.Name);
                    //dd.Undeploy(item);
                    //System.Console.Out.WriteLine("Importing Policy ({0}) : (RuleStoreRuleSetAlreadyPublishedException) Successfully undeployed RulesetInfo {1}, next remove policy ", policy.Name, item.Name);
                    bool toDeploy = false;
                    try
                    {
                        targetRuleStore.Remove(policy);
                    }
                    catch (Microsoft.RuleEngine.RuleStoreRuleSetDeployedException ex) {
                        dd.Undeploy(item);
                        targetRuleStore.Remove(policy);
                        toDeploy = true;
                    }
                    targetRuleStore.Add(policy);
                    targetRuleStore.Publish(policy);
                    if (toDeploy) {
                        dd.Deploy(item);
                        toDeploy = false;
                    }
                }
                catch (Microsoft.RuleEngine.RuleStoreRuleSetDeployedException e) {
                    System.Console.Out.WriteLine("Importing Policy ({0}) : (RuleStoreRuleSetDeployedException) Undeploying RulesetInfor {1}", policy.Name, item.Name);
                    dd.Undeploy(item); 
                    targetRuleStore.Remove(policy);
                    targetRuleStore.Add(policy);
                    dd.Deploy(item);
                }
            }
        }

        // for export 
        private static void CopyPolicies(RuleStore sourceRuleStore, RuleStore targetRuleStore)
        {
            RuleSetInfoCollection rulesetInfoList = sourceRuleStore.GetRuleSets(RuleStore.Filter.All);
            foreach (RuleSetInfo item in rulesetInfoList)
            {
                RuleSet policy = sourceRuleStore.GetRuleSet(item);
                    targetRuleStore.Add(policy);
            }
        }

        private static void ImportVocabularies()
        {
            // RuleSetDeploymentDriver has the following important methods            
            Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver dd = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver();

            // SqlRuleStore - gives access t0 the rule engine database
            SqlRuleStore sqlRuleStore = (SqlRuleStore)dd.GetRuleStore();

            // Establish the export file
            FileRuleStore fileRuleStore = new FileRuleStore(vocabExportPath);

            CopyVocabularies(fileRuleStore, sqlRuleStore,dd);
        }

        public static void ExportVocabularies()
        {
            // RuleSetDeploymentDriver has the following important methods            
            Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver dd = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver();

            // SqlRuleStore - gives access t0 the rule engine database
            SqlRuleStore sqlRuleStore = (SqlRuleStore)dd.GetRuleStore();

            // Establish the export file
            FileRuleStore fileRuleStore = new FileRuleStore(vocabExportPath);

            CopyVocabularies(sqlRuleStore, fileRuleStore);            
        }

        // for export
        private static void CopyVocabularies(RuleStore sourceRuleStore, RuleStore targetRuleStore)
        {
            VocabularyInfoCollection vocabInfoList = sourceRuleStore.GetVocabularies(RuleStore.Filter.All);
            foreach (VocabularyInfo vocabInfoItem in vocabInfoList)
            {
                Vocabulary vocabItem = sourceRuleStore.GetVocabulary(vocabInfoItem);

                string[] excludedVocabularyNames = { "Predicates", "Common Values", "Common Sets", "Functions" };
                if (!excludedVocabularyNames.Contains(vocabItem.Name))
                {
                        targetRuleStore.Add(vocabItem);
                }
            }
        }

        // for import 
        private static void CopyVocabularies(RuleStore sourceRuleStore, RuleStore targetRuleStore, Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver dd)
        {
            VocabularyInfoCollection vocabInfoList = sourceRuleStore.GetVocabularies(RuleStore.Filter.All);
            foreach (VocabularyInfo vocabInfoItem in vocabInfoList)
            {
                Vocabulary vocabItem = sourceRuleStore.GetVocabulary(vocabInfoItem);

                string[] excludedVocabularyNames = { "Predicates", "Common Values", "Common Sets", "Functions" };
                if (!excludedVocabularyNames.Contains(vocabItem.Name))
                {
                    try
                    {
                        targetRuleStore.Add(vocabItem);
                    }
                    catch (Exception e)
                    {
                        //targetRuleStore.Remove(vocabItem); 
                        //targetRuleStore.Add(vocabItem);
                        //targetRuleStore.Publish(vocabItem);
                    }
                }
            }
        }
    }
}
