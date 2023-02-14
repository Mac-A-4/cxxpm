
namespace cxxpm.Utility {

    internal static class DirectoryUtil {

        public delegate T HandlerForResult<T>();

        public static T In<T>(string directory, HandlerForResult<T> handler) {
            var previous = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(directory);
            try {
                return handler();
            } finally {
                Directory.SetCurrentDirectory(previous);
            }
        }

        public delegate void Handler();

        public static void In(string directory, Handler handler) {
            In(directory, () => {
                handler();
                return true;
            });
        }

    }

}
