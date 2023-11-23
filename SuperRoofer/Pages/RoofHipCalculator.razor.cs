
using global::Microsoft.AspNetCore.Components;

using System;
using  static  SuperRoofer . Pages . RoofHipCalculator ;

namespace SuperRoofer.Pages
{
    public partial class RoofHipCalculator
    {
        RoofGeometry _RoofGeometry = new RoofGeometry();
        private void CalculateGeometry()
        {
            _RoofGeometry.Calculate(_RoofGeometry);
        }

        /// <summary>
        /// This class:
        /// Describes- a rectangular hipped roof geometry (as a set of numerical fields only) 
        /// Methods- Calculating missing fields, of particular interest, hip length
        /// </summary>
        public class RoofGeometry
        {
            /// <summary>
            /// Ctor: Default values from instructions
            /// </summary>
            public RoofGeometry()
            {
                Length = 10;
                Width = 5;
                SlopeAngle = 45;
                EndSlopeAngle = 35;
            }

            /// <summary>
            /// The length of the roof, as measured at base, in m
            /// </summary>
            [Parameter]
            public double Length { get; set; }

            /// <summary>
            /// The Width of the roof, as measured at base, in m
            /// </summary>
            [Parameter]
            public double Width { get; set; }

            /// <summary>
            /// The height of the roof, as measured in vertical, in m
            /// </summary>
            [Parameter]
            public double Hieght { get; set; }

            /// <summary>
            /// The slope of the hipped end roof, as measured through 
            /// a vertical plane ortho to the end wall, in deg
            /// </summary>
            [Parameter]
            public double EndSlopeAngle { get; set; }

            /// <summary>
            /// The slope of the main roof, as measured through
            /// a vertical plane ortho to the edge wall, in deg
            /// </summary>
            [Parameter]
            public double SlopeAngle { get; set; }

            /// <summary>
            /// The length of the apex ridge of the roof, in m
            /// </summary>
            [Parameter]
            public double RidgeLength { get; set; }

            /// <summary>
            /// The length of the hip of the roof, in m
            /// </summary>
            [Parameter]
            public double HipLength { get; set; }

            /// <summary>
            /// Method for calculating the values of the remaining roof geometry.
            /// </summary>
            /// <param name = "roofGeo"></param>
            public void Calculate(RoofGeometry roofGeo)
            {
                // send all the inputs through validation guard statements
                // Currently doesn't do zero degree slopes (gable end roof)
                foreach (var field in this.GetType().GetFields())
                {
                    if (field.FieldType == typeof(double))
                    {
                        double value = (double)field.GetValue(this);
                        if (!(value > 0))
                        {
                            return;
                        }
                    }
                }

                //avoiding an edge case that will break the maths & provides little user benefit
                if (Width > Length)
                {
                    return;
                }

                //Calculate the missing values, these depend sequentially
                Hieght = CalculateHieght(Width, SlopeAngle);
                RidgeLength = CalculateRidgeLength(Length, Hieght, EndSlopeAngle);
                HipLength = CalculateHipLength(Hieght, Width, RidgeLength, Length);
            }

            /// <summary>
            /// the ridge Length is calculated from trig of a vertical triangle through the mid section
            /// </summary>
            /// <param name = "width"></param>
            /// <param name = "slopeAngle"></param>
            /// <returns></returns>
            private double CalculateHieght(double width, double slopeAngle)
            {
                return width / 2 * Math.Tan(slopeAngle * Math.PI / 180);
            }

            /// <summary>
            /// the ridge Length is calculated from trig of a vertical triangle through the hipped gable end.
            /// At shallow end slope angles, it is possible for the ridge length to be 0, but not less than!
            ///  
            /// </summary>
            /// <param name = "length"></param>
            /// <param name = "hieght"></param>
            /// <param name = "endSlopeAngle"></param>
            /// <returns></returns>
            private double CalculateRidgeLength(double length, double hieght, double endSlopeAngle)
            {
                return Math.Max(length - 2 * (hieght / Math.Tan(endSlopeAngle * Math.PI / 180)), 0);
            }

            /// <summary>
            /// The hip length is calculated using pythag of the 3 square dimensions the hip extends through
            /// TODO - incorrect answer when RidgeLength = 0 .
            /// </summary>
            /// <param name = "hieght"></param>
            /// <param name = "width"></param>
            /// <param name = "ridgeLength"></param>
            /// <param name = "length"></param>
            /// <returns></returns>
            private double CalculateHipLength(double hieght, double width, double ridgeLength, double length)
            {
                return Math.Sqrt(Math.Pow(hieght, 2) + Math.Pow((width / 2), 2) + Math.Pow((length - ridgeLength) / 2, 2));
            }
        }
    }
}