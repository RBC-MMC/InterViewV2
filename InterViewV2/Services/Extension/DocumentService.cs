using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using InterViewV2.Services.Interfaces;

namespace InterViewV2.Services.Extension
{
        public  class DocumentService : IDocumentService
        {
            public byte[] ReplaceInWord(byte[] document, Dictionary<string, string> model)
            {
                using var stream = new MemoryStream();
                stream.Write(document, 0, document.Length);
                using WordprocessingDocument doc = WordprocessingDocument.Open(stream, true);
                var body = doc.MainDocumentPart.Document.Body;

                var texts = body.Descendants<Text>();

                foreach (var key in model.Keys)
                {
                    var maskedKey = MaskKey(key);
                    foreach (var text in texts.Where(x => x.Text.Contains(maskedKey)))
                    {
                        if (model[key] != "check_true" && model[key] != "check_false")
                        {
                            text.Text = text.Text.Replace(maskedKey, model[key]);
                        }
                        else
                        {
                            text.Text = text.Text.Replace(maskedKey, model[key] == "check_true" ? "☒" : "☐");
                        }
                    }
                }
                doc.Close();
                return stream.ToArray();
            }

            public byte[] ReplaceInWord(byte[] document, string tableName, IEnumerable<Dictionary<string, string>> items)
            {
                using var stream = new MemoryStream();
                stream.Write(document, 0, document.Length);
                using var doc = WordprocessingDocument.Open(stream, true);
                var body = doc.MainDocumentPart.Document.Body;
                foreach (var table in body.Descendants<Table>().Where(x => x.InnerText.StartsWith(tableName)))
                {
                    var tableNameRow = table.ChildElements.First(x => x.GetType() == typeof(TableRow));
                    table.RemoveChild(tableNameRow);
                    var rows = table.ChildElements.Where(x => x.GetType() == typeof(TableRow)).ToList();
                    foreach (var item in items)
                    {
                        foreach (var row in rows)
                        {
                            var newRow = row.CloneNode(true);
                            var texts = newRow.Descendants<Text>();

                            foreach (var key in item.Keys)
                            {
                                var maskedKey = MaskKey(key);
                                foreach (var text in texts.Where(x => x.Text.Contains(maskedKey)))
                                {
                                    text.Text = text.Text.Replace(maskedKey, item[key]);
                                }
                            }

                            table.Append(newRow);
                        }
                    }

                    foreach (var row in rows)
                    {
                        table.RemoveChild(row);
                    }
                }
                doc.Close();
                return stream.ToArray();
            }

            private string MaskKey(string key)
            {
                return string.Format("w{0}w", key);
            }
        }
    }

