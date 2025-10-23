using NJsonSchema.CodeGeneration.TypeScript;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ClientGenerator.Api.ProLab
{
  class Program
  {
    static async Task Main(string[] args)
    {
      if (args.Length != 3)
        throw new ArgumentException("Expecting 3 arguments: URL, generatePath, language");

      var url = args[0];
      var generatePath = Path.Combine(Directory.GetCurrentDirectory(), args[1]);
      var language = args[2];

      if (language != "TypeScript" && language != "CSharp")
        throw new ArgumentException("Invalid language parameter; valid values are TypeScript and CSharp");

      if (language == "TypeScript")
        await GenerateTypeScriptClient(url, generatePath);
      else
        await GenerateCSharpClient(url, generatePath);
    }

    async static Task GenerateTypeScriptClient(string url, string generatePath) =>
        await GenerateClient(
            document: await OpenApiDocument.FromUrlAsync(url),
            generatePath: generatePath,
            generateCode: (document) =>
            {
              var settings = new TypeScriptClientGeneratorSettings();

              //settings.TypeScriptGeneratorSettings.
              settings.Template = TypeScriptTemplate.Axios;
              //settings.UseGetBaseUrlMethod = false;
              settings.TypeScriptGeneratorSettings.NullValue = TypeScriptNullValue.Null;
              settings.TypeScriptGeneratorSettings.MarkOptionalProperties = false;
              // wonder  why it was here
              settings.UseTransformResultMethod = true;
              settings.ClientBaseClass = "ClientsBase";

              //settings.TypeScriptGeneratorSettings.ExtensionCode="ClientsBase.ts";
              //settings.TypeScriptGeneratorSettings.ExtendedClasses=new List<string>() {"//helloworld" }.ToArray();


              settings.TypeScriptGeneratorSettings.TypeStyle = TypeScriptTypeStyle.Interface;
              settings.TypeScriptGeneratorSettings.TypeScriptVersion = 5M;
              settings.TypeScriptGeneratorSettings.DateTimeType = TypeScriptDateTimeType.Date;

              var generator = new TypeScriptClientGenerator(document, settings);
              var code = generator.GenerateFile();

              return code;
            }
        );

    async static Task GenerateCSharpClient(string url, string generatePath) =>
        await GenerateClient(
            document: await OpenApiDocument.FromUrlAsync(url),
            generatePath: generatePath,
            generateCode: (document) =>
            {
              var settings = new CSharpClientGeneratorSettings
              {
                UseBaseUrl = false
              };

              var generator = new CSharpClientGenerator(document, settings);
              var code = generator.GenerateFile();
              return code;
            }
        );

    private async static Task GenerateClient(OpenApiDocument document, string generatePath, Func<OpenApiDocument, string> generateCode)
    {
      Console.WriteLine($"Generating {generatePath}...");

      var code = generateCode(document);

      await File.WriteAllTextAsync(generatePath, code);
    }
  }
}
