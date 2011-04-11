using System;
using System.IO;
using ErikEJ.SqlCeScripting;

namespace SchemaCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
                throw new ApplicationException("You must specify a target to update and source to compare from and a filename to save the differences to");

            var source = string.Format(@"Data Source={0}", args[0].Replace("-source:", ""));
            var target = string.Format(@"Data Source={0}", args[1].Replace("-target:", ""));
            var outputPath = args[2].Replace("-outputPath:", "");
            CreateSqlCeDifference(source, target, outputPath);
        }

        private static void CreateSqlCeDifference(string source, string target, string outputPath)
        {
            using (IRepository sourceRepository = new DB4Repository(source))
            {
                var generator = new Generator4(sourceRepository);
                using (IRepository targetRepository = new DB4Repository(target))
                {
                    SqlCeDiff.CreateDiffScript(sourceRepository, targetRepository, generator);

                    OutputDiffScript(generator.GeneratedScript, outputPath);
                }
            }
        }

        private static void OutputDiffScript(string diffScript, string outputPath)
        {
            string explain = @"-- This database diff script contains the following objects:
-- - Tables:  Any that are not in the destination
-- -          (tables that are only in the destination are not dropped)
-- - Columns: Any added, deleted, changed columns for existing tables
-- - Indexes: Any added, deleted indexes for existing tables
-- - Foreign keys: Any added, deleted foreign keys for existing tables
-- ** Make sure to test against a production version of the destination database! ** " + Environment.NewLine + Environment.NewLine;
            
            var tw = new StreamWriter(outputPath);
            tw.Write(explain + diffScript);
            tw.Flush();
            tw.Close();
        }
    }
}
