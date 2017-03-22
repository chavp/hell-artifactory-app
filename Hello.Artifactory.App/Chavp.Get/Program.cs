using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chavp.Get
{
    using Appccelerate.CommandLineParser;
    using RestSharp;
    using RestSharp.Authenticators;
    using System.IO;

    class Program
    {
        static string ArtsRepoUrl = "http://172.16.9.101:8081";
        static string ProgramsFolder = @"C:\Program Files\chavp\hello-artifactory-app";
        static string HelloAppName = "hello.artifactory.app";

        static void Main(string[] args)
        {
            string appName = string.Empty;
            string ver = string.Empty;

            var configuration = CommandLineParserConfigurator
                .Create()
                .WithNamed("a", v => appName = v)
                        .HavingLongAlias("app")
                        .Required()
                        .RestrictedTo(
                            HelloAppName
                        )
                        .DescribedBy("application name", "Specifies target app.")
                .WithNamed("v", v => ver = v)
                        .HavingLongAlias("ver")
                        .Required()
                        .DescribedBy("version", "specifies version")
                .BuildConfiguration();

            var parser = new CommandLineParser(configuration);

            var parseResult = parser.Parse(args);

            if (!parseResult.Succeeded)
            {
                Usage usage = new UsageComposer(configuration).Compose();
                // Console.WriteLine(parseResult.Message);
                Console.WriteLine(parseResult.Message);
                Console.WriteLine("usage: ", usage.Arguments);
                Console.WriteLine("options: ");
                Console.WriteLine(usage.Options.IndentBy(4));
                Console.WriteLine();

                return;
            }

            var artPath = string.Format(
                "artifactory/example-repo-local/chavp/hello-artifactory-app/hello-artifactory-app-{0}.exe",
                ver);

            appName = string.Format("{0}.exe", appName);

            install(artPath, appName);
        }

        private static void install(string path, string appName)
        {
            var client = new RestClient(ArtsRepoUrl);
            client.Authenticator = new HttpBasicAuthenticator("admin", "password");

            var request = new RestRequest(path, Method.GET);
            Console.WriteLine("Begin download: " + path);
            var res = client.Execute(request);
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("{0} path {1}", res.StatusDescription, path);
            }
            else
            {
                if (!Directory.Exists(ProgramsFolder))
                {
                    Directory.CreateDirectory(ProgramsFolder);
                }

                System.IO.File.WriteAllBytes(System.IO.Path.Combine(ProgramsFolder, appName), res.RawBytes);
                Console.WriteLine("Successfully install " + appName);
            }
        }
    }
}
