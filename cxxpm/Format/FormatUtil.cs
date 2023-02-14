using Newtonsoft.Json;

namespace cxxpm.Format {

    class FormatUtil<T> {

        public static T Load(string path) {
            var data = Read(path);
            var result = JsonConvert.DeserializeObject<T>(data);
            if (result == null) {
                throw new InvalidDataException("Failed to deserialize JSON");
            }
            return result;
        }

        public static void Save(string path, T value) {
            var data = JsonConvert.SerializeObject(value, Formatting.Indented);
            if (data == null) {
                throw new ArgumentException("Failed to serialize JSON");
            }
            Write(path, data);
        }

        private static string Read(string path) {
            return File.ReadAllText(path);
        }

        private static void Write(string path, string value) {
            File.WriteAllText(path, value);
        }

    }

}
