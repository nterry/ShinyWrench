using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebApplication1.Lib.Domain.Java.JavaModels;

namespace WebApplication1.Lib.Domain.Java.JavaParsers
{
    class JavaParser
    {
        private const string classParser = @"^\s?(?:(?:(public|private)\s+)|(\s?))class\s+([a-zA-Z][a-zA-Z0-9]*)\s?\{?\s?$";
        private const string fieldParser = @"^\s?(?:(?:(public|private)\s+)|(\s?))([a-zA-Z0-9_-]*(?:(?:\[\])|(?:\<[a-zA_Z0-9_-]*\>)){0,1})\s+([a-zA-Z0-9_-]*)\s?(?:;|=\s?([a-zA-Z0-9])*\s?;)$";
        private const string methodParser = @"^\s?(?:(?:(public|private)\s+)|(\s?))([a-zA-Z0-9_-]*(?:(?:\[\])|(?:\<[a-zA_Z0-9_-]*\>)){0,1})(?:[a-zA-Z0-9]*)\s+([a-zA-Z0-9_-]*)\s?\((?:\s?[a-zA-Z0-9]+\s+[a-zA-Z0-9_-]+\s?,?)*\)\s?\{?$";

        public static JavaClass ParseClass(string[] javaClass)
        {
            var @class = new JavaClass();
            var fieldList = new List<JavaField>();
            var methodList = new List<JavaMethod>();

            foreach (var line in javaClass)
            {
                var trimmedLine = Sanitize(line);
                
                var classMatch = Regex.Match(trimmedLine, classParser);
                var fieldMatch = Regex.Match(trimmedLine, fieldParser);
                var methodMatch = Regex.Match(trimmedLine, methodParser);

                if (classMatch.Success)
                {
                    @class.ClassVisibility = classMatch.Groups[1].Value;
                    @class.ClassName = classMatch.Groups[3].Value;
                }

                else if (fieldMatch.Success)
                {
                    fieldList.Add(new JavaField { Name = fieldMatch.Groups[4].Value, Type = fieldMatch.Groups[3].Value, Visibility = fieldMatch.Groups[1].Value });
                }

                else if (methodMatch.Success)
                {
                    methodList.Add(new JavaMethod { Name = methodMatch.Groups[4].Value, Type = methodMatch.Groups[3].Value, Visibility = methodMatch.Groups[1].Value });
                }
            }

            @class.Fields = fieldList.ToArray();
            @class.Methods = methodList.ToArray();
            return @class;
        }

        private static string Sanitize(string unsanitizedString)
        {
            var trimmedLine = unsanitizedString.Trim();
            trimmedLine = trimmedLine.Replace("\n", String.Empty);
            trimmedLine = trimmedLine.Replace("\r", String.Empty);
            trimmedLine = trimmedLine.Replace("\t", String.Empty);

            return trimmedLine;
        }

    }
}
