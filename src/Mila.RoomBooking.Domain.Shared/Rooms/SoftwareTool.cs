using System;

namespace UniversityBooking.Rooms
{
    /// <summary>
    /// Software tools available in lab rooms
    /// </summary>
    [Flags]
    public enum SoftwareTool
    {
        None = 0,
        VSCode = 1 << 0,
        ClaudeDesktop = 1 << 1,
        MicrosoftOffice = 1 << 2,
        AdobeCreativeSuite = 1 << 3,
        MATLAB = 1 << 4,
        Python = 1 << 5,
        RStudio = 1 << 6,
        SPSS = 1 << 7,
        AutoCAD = 1 << 8,
        Unity = 1 << 9,
        AndroidStudio = 1 << 10,
        XCode = 1 << 11,
        VisualStudio = 1 << 12,
        Eclipse = 1 << 13,
        ArcGIS = 1 << 14,
        SolidWorks = 1 << 15
    }
}