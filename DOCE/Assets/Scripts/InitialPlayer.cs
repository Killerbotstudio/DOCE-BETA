public static class InitialPlayer
{
    private static PlayerInfo ini;
    private static int index;

    public static PlayerInfo GetSetIni
    {
        get
        {
            return ini;
        }
        set
        {
            ini = value;
        }
    }

    public static int PlayerIndex
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }
}
