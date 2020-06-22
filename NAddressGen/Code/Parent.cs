#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 9:51
#endregion

using System;

namespace NAddressGen.Code
{
    public class Parent
    {

        public Guid AoGuid { get; set; }
        public string OffName { get; set; }
        public Guid? ParentGuid { get; set; }
        public string ShortName { get; set; }
    }
}
