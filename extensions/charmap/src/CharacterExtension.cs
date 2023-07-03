namespace gehtsoft.xce.extension.charmap_impl
{
    internal static class CharacterExtension
    {
        public static bool IsSpecial(this char c) => c < 0x20 || c >= 0x7f && c < 0xa0;
    }
}

