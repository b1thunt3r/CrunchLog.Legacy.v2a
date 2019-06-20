﻿using Bit0.CrunchLog.Config;
using Bit0.CrunchLog.Extensions;
using Bit0.CrunchLog.Template;
using Bit0.CrunchLog.Template.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUglify;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ScribanTemplate = Scriban.Template;

namespace Bit0.CrunchLog.Plugins.ScribanEngine
{
    internal class ScribanTemplateEngine : ITemplateEngine
    {
        private readonly CrunchSite _siteConfig;
        private readonly ILogger<ScribanEnginePlugin> _logger;
        private readonly IContentProvider _contentProvider;

        public ScribanTemplateEngine(CrunchSite siteConfig, ILogger<ScribanEnginePlugin> logger, IContentProvider contentProvider)
        {
            _siteConfig = siteConfig;
            _logger = logger;
            _contentProvider = contentProvider;
        }

        public void PreProcess() { }
        public void PostProcess(CrunchSite siteConfig, Theme theme) { }

        public void Render(ITemplateModel model)
        {
            var outputDir = _siteConfig.Paths.OutputPath;

            if (model is PostRedirectTemplateModel redirect)
            {
                outputDir = outputDir.CombineDirPath(redirect.RedirectUrl.Substring(1));
                Render(model, "Redirect", outputDir.CombineFilePath(".html", "index"));
            }
            else if (model is PostTemplateModel post)
            {
                outputDir = !post.IsDraft ? outputDir.CombineDirPath(post.Permalink.Substring(1)) : outputDir.CombineDirPath("draft", post.Id);
                Render(model, "Post", outputDir.CombineFilePath(".html", "index"));
            } else if (model is NotFoundTemplateModel)
            {
                outputDir = outputDir.CombineDirPath("404");
                Render(model, "404", outputDir.CombineFilePath(".html", "index"));
            }
            else if (model is PostListTemplateModel)
            {
                outputDir = outputDir.CombineDirPath(model.Permalink.Replace("//", "/").Substring(1));
                Render(model, "List", outputDir.CombineFilePath(".html", "index"));
            }
        }

        private void Render<TModel>(TModel model, String viewName, FileInfo outputFile) where TModel : ITemplateModel
        {
            if (!outputFile.Directory.Exists)
            {
                _logger.LogTrace($"Create directory: {outputFile.Directory.FullName}");
                outputFile.Directory.Create();
            }

            _logger.LogTrace($"Render template from view: {viewName}");

            using (var sw = outputFile.CreateText())
            {
                var content = RenderView(viewName, model);
                sw.WriteLine(content);
            }
        }

        private String RenderView<TModel>(String viewName, TModel model) where TModel : ITemplateModel
        {
            var context = new TemplateContext
            {
                TemplateLoader = new CrunchTemplateLoader(_siteConfig.Theme.PackFile.Directory),
                MemberRenamer = member => member.Name,
                MemberFilter = member => member is PropertyInfo
            };

            var contextObj = new ScriptObject();
            contextObj["site"] = _siteConfig.GetModel(_contentProvider);
            contextObj["model"] = model;
            contextObj["title"] = model.Title;
            contextObj.SetValue("dump", new DumpFunction(), true);

            context.PushGlobal(contextObj);

            var template = ScribanTemplate.Parse("{{ include '" + viewName + "' }}");
            var content = FormatHtml(template.Render(context));
            return content;
        }

        private static String FormatHtml(String content)
        {
            var result = Uglify.Html(content);
            content = result.Code;
            return content;
        }
    }

    public class DumpFunction : ScriptObject, IScriptCustomFunction
    {
        public Object Invoke(TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement)
        {
            return $"<pre>{JsonConvert.SerializeObject(arguments, Formatting.Indented)}</pre>";
        }

        public ValueTask<Object> InvokeAsync(TemplateContext context, ScriptNode callerContext, ScriptArray arguments, ScriptBlockStatement blockStatement)
        {
            return new ValueTask<Object>(Invoke(context, callerContext, arguments, blockStatement));
        }
    }
}