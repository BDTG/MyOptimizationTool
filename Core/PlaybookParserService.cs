using MyOptimizationTool.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using YamlDotNet.Serialization;

namespace MyOptimizationTool.Core
{
    public class PlaybookParserService
    {
        // Hàm trợ giúp để đọc một Element từ XML một cách an toàn (hỗ trợ CDATA)
        private string GetXmlElementValue(XElement parent, string elementName, string defaultValue = "")
        {
            var element = parent.Element(elementName);
            if (element?.FirstNode is XCData cdata)
            {
                return cdata.Value;
            }
            return element?.Value ?? defaultValue;
        }

        // Hàm trợ giúp để đọc một Attribute từ XML một cách an toàn
        private string GetXmlAttributeValue(XElement element, string attributeName, string defaultValue = "")
        {
            return element.Attribute(attributeName)?.Value ?? defaultValue;
        }

        // Phương thức async để parse playbook từ root path
        public async Task<Playbook> ParsePlaybookAsync(string rootPath)
        {
            // Validation cơ bản cho path (ngăn traversal và kiểm tra tồn tại)
            if (string.IsNullOrWhiteSpace(rootPath) || rootPath.Contains("..") || !Directory.Exists(rootPath))
            {
                Debug.WriteLine($"Invalid root path: {rootPath}");
                return new Playbook
                {
                    RootPath = rootPath,
                    Name = "Lỗi",
                    Description = "Đường dẫn không hợp lệ hoặc không tồn tại."
                };
            }

            var playbook = new Playbook { RootPath = rootPath };
            var confPath = Path.Combine(rootPath, "playbook.conf");

            if (!File.Exists(confPath))
            {
                Debug.WriteLine($"Config file not found: {confPath}");
                playbook.Name = "Lỗi";
                playbook.Description = "Không tìm thấy file playbook.conf.";
                return playbook;
            }

            try
            {
                // Async load XML file
                using var stream = new FileStream(confPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                XDocument doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
                XElement? rootElement = doc.Element("Playbook");

                if (rootElement == null)
                {
                    Debug.WriteLine("Missing root element 'Playbook' in config.");
                    playbook.Description = "File playbook.conf không có phần tử gốc 'Playbook'.";
                    return playbook;
                }

                // Đọc thông tin playbook cơ bản
                playbook.Name = GetXmlElementValue(rootElement, "Name", "Unnamed Playbook");
                playbook.Author = GetXmlElementValue(rootElement, "Username", "Unknown Author");
                playbook.Description = GetXmlElementValue(rootElement, "Description", "No description provided.");

                // Đọc Feature Pages
                var featurePagesElement = rootElement.Element("FeaturePages");
                if (featurePagesElement != null)
                {
                    foreach (var pageElement in featurePagesElement.Elements())
                    {
                        FeaturePage? featurePage = null;

                        switch (pageElement.Name.LocalName)
                        {
                            case "RadioPage":
                                var radioPage = new RadioPage { DefaultOption = GetXmlAttributeValue(pageElement, "DefaultOption") };
                                radioPage.Options.AddRange(pageElement.Element("Options")?.Elements("RadioOption")
                                    .Select(opt => new PageOption { Text = GetXmlElementValue(opt, "Text"), Name = GetXmlElementValue(opt, "Name") })
                                    ?? Enumerable.Empty<PageOption>());
                                featurePage = radioPage;
                                break;

                            case "CheckboxPage":
                                var checkboxPage = new CheckboxPage();
                                checkboxPage.Options.AddRange(pageElement.Element("Options")?.Elements("CheckboxOption")
                                    .Select(opt => new PageOption { Text = GetXmlElementValue(opt, "Text"), Name = GetXmlElementValue(opt, "Name") })
                                    ?? Enumerable.Empty<PageOption>());
                                featurePage = checkboxPage;
                                break;

                            case "RadioImagePage":
                                var radioImagePage = new RadioImagePage { DefaultOption = GetXmlAttributeValue(pageElement, "DefaultOption") };
                                radioImagePage.Options.AddRange(pageElement.Element("Options")?.Elements("RadioImageOption")
                                    .Select(opt => new RadioImageOption { Text = GetXmlElementValue(opt, "Text"), Name = GetXmlElementValue(opt, "Name"), FileName = GetXmlElementValue(opt, "FileName") })
                                    ?? Enumerable.Empty<RadioImageOption>());
                                featurePage = radioImagePage;
                                break;
                        }

                        if (featurePage != null)
                        {
                            featurePage.Description = GetXmlAttributeValue(pageElement, "Description");
                            featurePage.IsRequired = GetXmlAttributeValue(pageElement, "IsRequired", "true") == "true";
                            featurePage.TopLineText = pageElement.Element("TopLine")?.Attribute("Text")?.Value;
                            featurePage.BottomLineText = pageElement.Element("BottomLine")?.Attribute("Text")?.Value;
                            featurePage.BottomLineLink = pageElement.Element("BottomLine")?.Attribute("Link")?.Value;
                            playbook.FeaturePages.Add(featurePage);
                        }
                    }
                }

                // Đọc các file tác vụ YAML async
                var tasksPath = Path.Combine(rootPath, "Configuration", "Tasks");
                if (Directory.Exists(tasksPath))
                {
                    var deserializer = new DeserializerBuilder().Build();
                    var taskFiles = Directory.GetFiles(tasksPath, "*.yml");

                    foreach (var file in taskFiles)
                    {
                        var fileContent = await File.ReadAllTextAsync(file);  // Async read
                        var yamlTasks = deserializer.Deserialize<List<Dictionary<string, object>>>(fileContent);

                        if (yamlTasks == null) continue;

                        foreach (var taskData in yamlTasks)
                        {
                            var task = new OptimizationTask
                            {
                                Name = taskData.ContainsKey("name") ? taskData["name"].ToString()! : "Unnamed Task",
                                Parameters = taskData
                            };

                            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
                            if (fileNameWithoutExt == "registry")
                                task.Type = TaskType.Registry;
                            else if (fileNameWithoutExt == "appx")
                                task.Type = TaskType.Appx;

                            playbook.Tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing playbook at {rootPath}: {ex.Message}");
                playbook.Name = "Lỗi Playbook";
                playbook.Description = $"Lỗi khi phân tích: {ex.Message}";
            }

            return playbook;
        }
    }
}
