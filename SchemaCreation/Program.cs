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

            CreateSqlDiffScript(source, target, outputPath);
        }

        private static void CreateSqlDiffScript(string source, string target, string outputPath)
        {
            using (IRepository sourceRepository = new DB4Repository(source))
            {
                var diffGenerator = new Generator4(sourceRepository);
                using (IRepository targetRepository = new DB4Repository(target))
                {
                    SqlCeDiff.CreateDiffScript(sourceRepository, targetRepository, diffGenerator);
                    OutputDiffScript(diffGenerator.GeneratedScript, outputPath);
                }
            }
        }

        private static void OutputDiffScript(string diffScript, string outputPath)
        {
            string warning = @"-- This database diff script contains the following objects:
                               -- - Tables:  Any that are not in the destination
                               -- -          (tables that are only in the destination are not dropped)
                               -- - Columns: Any added, deleted, changed columns for existing tables
                               -- - Indexes: Any added, deleted indexes for existing tables
                               -- - Foreign keys: Any added, deleted foreign keys for existing tables
                               -- ** Make sure to test against a production version of the destination database! ** " + Environment.NewLine + Environment.NewLine;

            if (File.Exists(outputPath))
                throw new ApplicationException("The output path already exists - please change the filename or delete the previous version");

            CreateDiffFile(outputPath, warning, diffScript);
        }

        private static void CreateDiffFile(string outputPath, string warning, string diffScript)
        {
            var outputWriter = new StreamWriter(outputPath);
            outputWriter.Write(warning + diffScript);
            outputWriter.Flush();
            outputWriter.Close();
        }
    }
}
