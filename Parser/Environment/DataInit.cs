namespace Parser
{
    //Класс инициализирующий массивы, используемые для отображения данных в listbox
    public class DataInit
    {

        public readonly int[] HR = new int[24];
        public readonly int[] MS = new int[60];

        public DataInit()
        {
            int i;
            for (i = 0; i < 24; i++)
            {
                HR[i] = i;
                MS[i] = i;
            }

            for (i = 24; i < 60; i++)
            {
                MS[i] = i;
            }
        }
    }
}
