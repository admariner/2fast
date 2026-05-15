using System.Collections.ObjectModel;

namespace Project2FA.Repository.Models
{
    public class DatafileModel
    {
        public ObservableCollection<TwoFACodeModel> Collection { get; set; }

        public ObservableCollection<CategoryModel> GlobalCategories { get; set; }
        public byte[] IV { get; set; }

        /// <summary>
        /// Random salt used for PBKDF2 key derivation (Version 3+).
        /// Null for legacy files (Version 0/1/2) which use the static salt.
        /// </summary>
        public byte[]? Salt { get; set; }

        public int Version { get; set; }
    }
}
