using AdvancedDealing.Utils;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace AdvancedDealing.Localization
{
    /// <summary>
    /// Loads editable localization files from UserData/AdvancedDealing/Localization.
    /// English is always used as a fallback for missing keys.
    /// </summary>
    public static class LocalizationManager
    {
        public const string DefaultLanguage = "en-US";

        private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

        private static Dictionary<string, string> _fallback = new(KeyComparer);
        private static Dictionary<string, string> _translations = new(KeyComparer);
        private static CultureInfo _formatCulture = CultureInfo.InvariantCulture;

        public static string CurrentLanguage { get; private set; } = DefaultLanguage;

        public static string LocalizationDirectory => Path.Combine(
            MelonEnvironment.UserDataDirectory,
            ModInfo.NAME,
            "Localization");

        public static void Initialize(string configuredLanguage)
        {
            try
            {
                Directory.CreateDirectory(LocalizationDirectory);

                EnsureBundledLanguageFile(DefaultLanguage);
                EnsureBundledLanguageFile("ru-RU");

                _fallback = LoadLanguage(DefaultLanguage, allowBundledFallback: true);
                CurrentLanguage = ResolveLanguage(configuredLanguage);
                _formatCulture = ResolveCulture(CurrentLanguage);

                _translations = CurrentLanguage.Equals(DefaultLanguage, StringComparison.OrdinalIgnoreCase)
                    ? new Dictionary<string, string>(_fallback, KeyComparer)
                    : LoadLanguage(CurrentLanguage, allowBundledFallback: false);

                if (_translations.Count == 0 && !CurrentLanguage.Equals(DefaultLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Warning("Localization", $"Language '{CurrentLanguage}' could not be loaded. Falling back to {DefaultLanguage}.");
                    CurrentLanguage = DefaultLanguage;
                    _formatCulture = ResolveCulture(DefaultLanguage);
                    _translations = new Dictionary<string, string>(_fallback, KeyComparer);
                }

                Logger.Msg("Localization", $"Loaded language: {CurrentLanguage}. Files: {LocalizationDirectory}");
            }
            catch (Exception ex)
            {
                CurrentLanguage = DefaultLanguage;
                _formatCulture = CultureInfo.InvariantCulture;
                _fallback = LoadBundledLanguage(DefaultLanguage);
                _translations = new Dictionary<string, string>(_fallback, KeyComparer);
                Logger.Error("Localization", "Localization initialization failed. Embedded English fallback will be used.", ex);
            }
        }

        public static string Get(string key, params object[] arguments)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            if (!_translations.TryGetValue(key, out string value) &&
                !_fallback.TryGetValue(key, out value))
            {
                Logger.Warning("Localization", $"Missing localization key: {key}");
                return $"[{key}]";
            }

            if (arguments == null || arguments.Length == 0)
            {
                return value;
            }

            try
            {
                return string.Format(_formatCulture, value, arguments);
            }
            catch (FormatException ex)
            {
                Logger.Error("Localization", $"Invalid format string for key '{key}': {value}", ex);
                return value;
            }
        }

        public static string Format(string format, params object[] arguments)
        {
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }

            try
            {
                return string.Format(_formatCulture, format, arguments);
            }
            catch (FormatException ex)
            {
                Logger.Error("Localization", $"Invalid runtime format string: {format}", ex);
                return format;
            }
        }

        private static string ResolveLanguage(string configuredLanguage)
        {
            string requested = configuredLanguage;

            if (string.IsNullOrWhiteSpace(requested) ||
                requested.Equals("auto", StringComparison.OrdinalIgnoreCase))
            {
                requested = CultureInfo.CurrentUICulture?.Name;
            }

            if (string.IsNullOrWhiteSpace(requested))
            {
                return DefaultLanguage;
            }

            requested = requested.Trim();

            if (requested.Equals("ru", StringComparison.OrdinalIgnoreCase))
            {
                requested = "ru-RU";
            }
            else if (requested.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                requested = DefaultLanguage;
            }

            string exactPath = GetLanguageFilePath(requested);
            if (File.Exists(exactPath))
            {
                return requested;
            }

            string languagePrefix = requested.Split('-', '_')[0];
            string matchingFile = null;

            foreach (string path in Directory.GetFiles(LocalizationDirectory, "*.json", SearchOption.TopDirectoryOnly))
            {
                string candidate = Path.GetFileNameWithoutExtension(path);
                string candidatePrefix = candidate.Split('-', '_')[0];

                if (candidatePrefix.Equals(languagePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    matchingFile = candidate;
                    break;
                }
            }

            return matchingFile ?? DefaultLanguage;
        }

        private static CultureInfo ResolveCulture(string language)
        {
            try
            {
                return CultureInfo.GetCultureInfo(language);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.InvariantCulture;
            }
        }

        private static Dictionary<string, string> LoadLanguage(string language, bool allowBundledFallback)
        {
            string path = GetLanguageFilePath(language);

            if (!File.Exists(path))
            {
                Logger.Warning("Localization", $"Localization file not found: {path}");
                return allowBundledFallback
                    ? LoadBundledLanguage(language)
                    : new Dictionary<string, string>(KeyComparer);
            }

            try
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                return result == null
                    ? new Dictionary<string, string>(KeyComparer)
                    : new Dictionary<string, string>(result, KeyComparer);
            }
            catch (Exception ex)
            {
                Logger.Error("Localization", $"Failed to load localization file: {path}", ex);
                return allowBundledFallback
                    ? LoadBundledLanguage(language)
                    : new Dictionary<string, string>(KeyComparer);
            }
        }

        private static Dictionary<string, string> LoadBundledLanguage(string language)
        {
            string resourceName = GetResourceName(language);

            try
            {
                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    Logger.Error("Localization", $"Embedded localization resource not found: {resourceName}");
                    return new Dictionary<string, string>(KeyComparer);
                }

                using StreamReader reader = new(stream, Encoding.UTF8, true);
                string json = reader.ReadToEnd();
                Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                return result == null
                    ? new Dictionary<string, string>(KeyComparer)
                    : new Dictionary<string, string>(result, KeyComparer);
            }
            catch (Exception ex)
            {
                Logger.Error("Localization", $"Failed to load embedded localization resource: {resourceName}", ex);
                return new Dictionary<string, string>(KeyComparer);
            }
        }

        /// <summary>
        /// Creates the editable language file on first launch and adds newly introduced
        /// bundled keys on later updates without replacing any user-edited values.
        /// </summary>
        private static void EnsureBundledLanguageFile(string language)
        {
            string destination = GetLanguageFilePath(language);
            Dictionary<string, string> bundled = LoadBundledLanguage(language);

            if (bundled.Count == 0)
            {
                return;
            }

            if (!File.Exists(destination))
            {
                WriteLanguageFile(destination, bundled);
                return;
            }

            try
            {
                string json = File.ReadAllText(destination, Encoding.UTF8);
                Dictionary<string, string> existingRaw =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                if (existingRaw == null)
                {
                    Logger.Warning("Localization", $"Localization file is empty and was not modified: {destination}");
                    return;
                }

                Dictionary<string, string> existing = new(existingRaw, KeyComparer);
                int added = 0;

                foreach (KeyValuePair<string, string> entry in bundled)
                {
                    if (!existing.ContainsKey(entry.Key))
                    {
                        existing.Add(entry.Key, entry.Value);
                        added++;
                    }
                }

                if (added > 0)
                {
                    WriteLanguageFile(destination, existing);
                    Logger.Msg("Localization", $"Added {added} new keys to {Path.GetFileName(destination)} without overwriting custom translations.");
                }
            }
            catch (Exception ex)
            {
                // A malformed user file must never be overwritten automatically.
                Logger.Error("Localization", $"Could not merge new keys into localization file: {destination}", ex);
            }
        }

        private static void WriteLanguageFile(string path, Dictionary<string, string> values)
        {
            string json = JsonConvert.SerializeObject(values, Formatting.Indented);
            string temporaryPath = path + ".tmp";

            File.WriteAllText(temporaryPath, json, new UTF8Encoding(false));

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(temporaryPath, path);
        }

        private static string GetLanguageFilePath(string language)
        {
            return Path.Combine(LocalizationDirectory, $"{language}.json");
        }

        private static string GetResourceName(string language)
        {
            return $"AdvancedDealing.Localization.{language}.json";
        }
    }
}
