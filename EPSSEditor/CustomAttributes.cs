using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
public class WidthAttribute : Attribute
{
    public WidthAttribute(int w)
    {
        Width = w;
    }
    public int Width { get; set; }
}


