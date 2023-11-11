using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VideoGenerator.Utils.Controls;

public class ImageEx : Image
{
    public ImageEx () : base()
    {
        SizeChanged += ImageEx_SizeChanged;
    }

    private void ImageEx_SizeChanged (object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
            ActualWidthValue = e.NewSize.Width;
        if (e.HeightChanged)
            ActualHeightValue = e.NewSize.Height;
    }

    public double ActualWidthValue
    {
        get => (double)GetValue(ActualWidthValueProperty);
        set => SetValue(ActualWidthValueProperty, value);
    }

    public static readonly DependencyProperty ActualWidthValueProperty = DependencyProperty.Register("ActualWidthValue", typeof(double), typeof(ImageEx), new PropertyMetadata(0.0));

    public double ActualHeightValue
    {
        get => (double)GetValue(ActualHeightValueProperty);
        set => SetValue(ActualHeightValueProperty, value);
    }

    public static readonly DependencyProperty ActualHeightValueProperty = DependencyProperty.Register("ActualHeightValue", typeof(double), typeof(ImageEx), new PropertyMetadata(0.0));
}
