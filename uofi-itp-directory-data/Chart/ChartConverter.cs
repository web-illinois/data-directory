using System.Text.Json;

namespace uofi_itp_directory_data.Orgchart {
    public static class ChartConverter {
        public static string ConvertToJson(string flatfile) {
            if (string.IsNullOrWhiteSpace(flatfile)) {
                return "File is empty";
            }

            var lines = flatfile.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) {
                return "File is empty";
            }
            var index = lines[0].StartsWith("title", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            var parentNode = ParseNode(lines[index]);
            if (!parentNode.ContainsKey("title")) {
                return "File must start with a title";
            }
            index++;
            parentNode["children"] = ParseLevel(lines, ref index, 1);
            if (parentNode["children"] == null || ((List<Dictionary<string, object>>)parentNode["children"]).Count == 0) {
                return "File only has one person -- please check on tab spacing";
            }
            return JsonSerializer.Serialize(parentNode);
        }

        private static List<Dictionary<string, object>> ParseLevel(string[] lines, ref int index, int depth) {
            var result = new List<Dictionary<string, object>>();

            while (index < lines.Length) {
                var currentLineDepth = GetDepth(lines[index]);
                if (currentLineDepth != depth) {
                    break;
                }
                var node = ParseNode(lines[index]);
                index++;

                if (index < lines.Length) {
                    var nextLineDepth = GetDepth(lines[index]);

                    if (nextLineDepth > depth) {
                        var children = ParseLevel(lines, ref index, nextLineDepth);
                        if (children.Count > 0) {
                            node["children"] = children;
                        }
                    }
                }
                result.Add(node);
            }
            return result;
        }

        private static Dictionary<string, object> ParseNode(string line) {
            var fields = TrimIndent(line).Split('\t', StringSplitOptions.None);
            var node = new Dictionary<string, object>();
            var keys = new[] { "title", "subtitle", "large", "weight" };

            for (var i = 0; i < keys.Length && i < fields.Length; i++) {
                var value = fields[i].Trim('"', ' ');
                if (!string.IsNullOrWhiteSpace(value)) {
                    if (value == "true" || value == "false" || value == "TRUE" || value == "FALSE") {
                        node[keys[i]] = bool.Parse(value);
                    } else if (int.TryParse(value, out var intValue)) {
                        node[keys[i]] = intValue;
                    } else {
                        node[keys[i]] = value;
                    }
                }
            }
            return node;
        }

        private static int GetDepth(string line) {
            var segments = line.Split('\t', StringSplitOptions.None);
            var depth = 0;
            foreach (var segment in segments) {
                if (string.IsNullOrWhiteSpace(segment)) {
                    depth++;
                    continue;
                }
                break;
            }
            return depth / 4;
        }

        private static string TrimIndent(string line) {
            var index = 0;

            while (index < line.Length && (line[index] == '\t' || line[index] == ' ')) {
                index++;
            }

            return line.Substring(index);
        }
    }
}
