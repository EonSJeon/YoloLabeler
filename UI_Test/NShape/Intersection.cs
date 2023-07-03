using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

// overlap
// https://stackoverflow.com/questions/11194152/is-there-a-more-efficient-way-to-detect-polygon-overlap-intersection-than-pathge

namespace UI_Test.NShape
{
    internal class Intersection
    {
        public static bool PointCollectionsOverlap_Fast(PointCollection area1, PointCollection area2)
        {
            if (area1.Count < 1 || area2.Count < 1) return false;

            for (int i = 0; i < area1.Count; i++)
            {
                for (int j = 0; j < area2.Count; j++)
                {
                    if (lineSegmentsIntersect(area1[i], area1[(i + 1) % area1.Count], area2[j], area2[(j + 1) % area2.Count]))
                    {
                        return true;
                    }
                }
            }

            if (PointCollectionContainsPoint(area1, area2[0]) ||
                PointCollectionContainsPoint(area2, area1[0]))
            {
                return true;
            }

            return false;
        }

        public static bool PointCollectionContainsPoint(PointCollection area, Point point)
        {
            Point start = new Point(-100, -100);
            int intersections = 0;

            for (int i = 0; i < area.Count; i++)
            {
                if (lineSegmentsIntersect(area[i], area[(i + 1) % area.Count], start, point))
                {
                    intersections++;
                }
            }

            return (intersections % 2) == 1;
        }

        private static double determinant(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.Y - vector1.Y * vector2.X;
        }

        private static bool lineSegmentsIntersect(Point _segment1_Start, Point _segment1_End, Point _segment2_Start, Point _segment2_End)
        {
            double det = determinant(_segment1_End - _segment1_Start, _segment2_Start - _segment2_End);
            double t = determinant(_segment2_Start - _segment1_Start, _segment2_Start - _segment2_End) / det;
            double u = determinant(_segment1_End - _segment1_Start, _segment2_Start - _segment1_Start) / det;
            return (t >= 0) && (u >= 0) && (t <= 1) && (u <= 1);
        }

        // ==========================================
        public static bool PointCollectionsOverlap_Slow(PointCollection area1, PointCollection area2)
        {
            PathGeometry pathGeometry1 = GetPathGeometry(area1);
            PathGeometry pathGeometry2 = GetPathGeometry(area2);
            bool result = pathGeometry1.FillContainsWithDetail(pathGeometry2) != IntersectionDetail.Empty;
            return result;
        }

        public static PathGeometry GetPathGeometry(PointCollection polygonCorners)
        {
            List<PathSegment> pathSegments = new List<PathSegment> { new PolyLineSegment(polygonCorners, true) };
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(new PathFigure(polygonCorners[0], pathSegments, true));
            return pathGeometry;
        }
    }
}
