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

            CopyPolicies(fileRuleStore, sqlRuleStore);
        }

        private static void ExportPolicies()
        {
            // RuleSetDeploymentDriver has the following important methods            
            Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver dd = new Microsoft.BizTalk.RuleEngineExtensions.RuleSetDeploymentDriver();

            // SqlRuleStore - gives access t0 the rule engine database
            SqlRuleStore sqlRuleStore = (SqlRuleStore)dd.GetRuleStore();

            // Establish the export file
            FileRuleStore fileRuleStore = new FileRuleStore(policyExportPath);

            CopyPolicies(sqlRuleStore, fileRuleStore);
        }

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

            CopyVocabularies(fileRuleStore, sqlRuleStore);
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
    }
}
