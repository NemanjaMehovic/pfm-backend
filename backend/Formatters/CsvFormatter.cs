using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace pfm.Formatter;

public class CsvFormatter : InputFormatter
{
    public CsvFormatter()
    {
        SupportedMediaTypes.Add("application/csv");
    }

    protected override bool CanReadType(Type type)
    {
        if (!type.GetInterfaces().Any(i => { return i == typeof(IEnumerable); }))
            return false;
        return type.IsGenericType && Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.Namespace == "pfm.Models").ToList().Contains(type.GetGenericArguments()[0]);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        try
        {
            string data = null;
            using (var streamReader = context.ReaderFactory(context.HttpContext.Request.Body, Encoding.UTF8))
            {
                data = await streamReader.ReadToEndAsync();
            }

            var type = context.ModelType.GetGenericArguments()[0];
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreBlankLines = false,
            };
            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            using (var reader = new StringReader(data))
            using (var csv = new CsvReader(reader, config))
            {
                var methodList = typeof(CsvContext).GetMethods().Where(method => method.Name == nameof(CsvContext.RegisterClassMap));
                MethodInfo methodInfo = null;
                foreach (var method in methodList)
                    if (method.IsGenericMethod)
                    {
                        methodInfo = method;
                        break;
                    }
                var allMaps = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType is not null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(ClassMap<>)).ToList();
                foreach (var t in allMaps)
                {
                    var method = methodInfo.MakeGenericMethod(t);
                    method.Invoke(csv.Context, null);
                }

                methodList = typeof(CsvReader).GetMethods().Where(method => method.Name == nameof(CsvReader.GetRecords));
                foreach (var method in methodList)
                    if (method.IsGenericMethod)
                    {
                        methodInfo = method;
                        break;
                    }
                methodInfo = methodInfo.MakeGenericMethod(type);
                var tmpList = ((IEnumerable<object>)methodInfo.Invoke(csv, null)).ToList();
                foreach (var tmp in tmpList)
                    list.Add(tmp);
            }
            return InputFormatterResult.Success(list);
        }
        catch (Exception e)
        {
            return InputFormatterResult.Failure();
        }
    }
}