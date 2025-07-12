// In folder: Core/PlaybookParserService.cs
using MyOptimizationTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq; // Cần cho việc đọc XML
using YamlDotNet.Serialization;
using System.Diagnostics; // Cần cho Debug

namespace MyOptimizationTool.Core
{
    public class PlaybookParserService
    {
        // Hàm trợ giúp để đọc một Element từ file XML một cách an toàn
        private string GetXmlElementValue(XElement parent, string elementName, string defaultValue = "")
        {
            // Xử lý cả CDATA
            var element = parent.Element(elementName);
            if (element?.FirstNode is XCData cdata)
            {
                return cdata.Value;
            }
            return element?.Value ?? defaultValue;
        }

        // Hàm trợ giúp để đọc một Attribute từ file XML một cách an toàn
        private string GetXmlAttributeValue(XElement element, string attributeName, string defaultValue = "")
        {
            return element.Attribute(attributeName)?.Value ?? defaultValue;
        }

        public Task<Playbook> ParsePlaybookAsync(string rootPath)
        {
            return Task.Run(() =>
            {
                var playbook = new Playbook { RootPath = rootPath };
                var confPath = Path.Combine(rootPath, "playbook.conf");

                if (!File.Exists(confPath))
                {
                    playbook.Name = "Lỗi";
                    playbook.Description = "Không tìm thấy file playbook.conf.";
                    return playbook;
                }

                try
                {
                    XDocument doc = XDocument.Load(confPath);
                    XElement? rootElement = doc.Element("Playbook");
                    if (rootElement == null) return playbook;

                    // --- Đọc thông tin playbook cơ bản ---
                    playbook.Name = GetXmlElementValue(rootElement, "Name", "Unnamed Playbook");
                    playbook.Author = GetXmlElementValue(rootElement, "Username", "Unknown Author");
                    playbook.Description = GetXmlElementValue(rootElement, "Description", "No description provided.");

                    // --- Đọc Feature Pages ---
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

                    // --- Đọc các file tác vụ YAML ---
                    var tasksPath = Path.Combine(rootPath, "Configuration", "Tasks");
                    if (Directory.Exists(tasksPath))
                    {
                        var deserializer = new DeserializerBuilder().Build();
                        var taskFiles = Directory.GetFiles(tasksPath, "*.yml");

                        foreach (var file in taskFiles)
                        {
                            var fileContent = File.ReadAllText(file);
                            var yamlTasks = deserializer.Deserialize<List<Dictionary<string, object>>>(fileContent);

                            if (yamlTasks == null) continue;

                            foreach (var taskData in yamlTasks)
                            {
                                var task = new OptimizationTask
                                {
                                    Name = taskData.ContainsKey("name") ? taskData["name"].ToString()! : "Unnamed Task",
                                    Parameters = taskData
                                };

                                if (Path.GetFileNameWithoutExtension(file).Equals("registry", StringComparison.OrdinalIgnoreCase))
                                    task.Type = TaskType.Registry;
                                else if (Path.GetFileNameWithoutExtension(file).Equals("appx", StringComparison.OrdinalIgnoreCase))
                                    task.Type = TaskType.Appx;

                                playbook.Tasks.Add(task);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error parsing playbook: {ex}");
                    playbook.Name = "Lỗi Playbook";
                    playbook.Description = $"Lỗi khi phân tích playbook.conf: {ex.Message}";
                }

                return playbook;
            });
        }
    }
}