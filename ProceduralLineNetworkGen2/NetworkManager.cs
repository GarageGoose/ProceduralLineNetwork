using System;
using System.Diagnostics;
using System.Numerics;

public class NetworkManager
{
    public class ElementsManager
    {
        public class ElementsDatabase
        {
            public OrderedDictionary<int, Element.Point> Points = new();
            public OrderedDictionary<int, Element.Line> Lines = new();

            public SortedDictionary<float, HashSet<int>>? MaxAngularDistanceAtPoint = new();

            public SortedDictionary<float, HashSet<int>>? MinAngularDistanceAtPoint = new();

            public SortedDictionary<int, HashSet<int>>? LineCountAtPoint = new();

            public SortedDictionary<int, HashSet<int>>? PointDensity = new();

            public SortedDictionary<string, HashSet<int>>? PointID = new();

            private int CurrUniqueElementKey = 0;
            public int NewUniqueElementKey()
            {
                CurrUniqueElementKey++;
                return CurrUniqueElementKey;
            }

            public ElementsDatabase StopTrackingAngularDistanceAtPoint(bool Min, bool Max)
            {
                if (Min)
                {
                    MaxAngularDistanceAtPoint = null;
                }
                if (Max)
                {
                    MaxAngularDistanceAtPoint = null;
                }
                return this;
            }

            public ElementsDatabase StopTrackingLineCountAtPoint()
            {
                LineCountAtPoint = null;
                return this;
            }

            public ElementsDatabase StopTrackingPointDensity()
            {
                PointDensity = null;
                return this;
            }
            public ElementsDatabase StopTrackingPointID()
            {
                PointID = null;
                return this;
            }
        }

        public class ElementsDatabaseHandler
        {
            ElementsDatabase DB;
            public ElementsDatabaseHandler(ElementsDatabase Database)
            {
                DB = Database;
            }

            public void AddLine(int ConnectingPointKeyA, int ConnectingPointKeyB, float AngleAtLineA, float AngleAtLineB)
            {
                int LineKey = DB.NewUniqueElementKey();
                DB.Lines.Add(DB.NewUniqueElementKey(), new(ConnectingPointKeyA, ConnectingPointKeyB));
                UpdatePointAddLine(ConnectingPointKeyA, LineKey, AngleAtLineA);
                UpdatePointAddLine(ConnectingPointKeyB, LineKey, (AngleAtLineB < MathF.PI) ? AngleAtLineB + MathF.PI : AngleAtLineB - MathF.PI);
            }
            private void UpdatePointAddLine(int PointKey, int LineID, float Angle)
            {
                DB.Points[PointKey].AddLine(LineID, Angle);

                if (DB.MaxAngularDistanceAtPoint != null)
                {
                    float OldMaxAngle = DB.Points[PointKey].MaxAngle.GetValueOrDefault();
                    float NewMaxAngle = 0;
                    if (DB.Points[PointKey].ConnectedLines.Count == 1)
                    {
                        NewMaxAngle = MathF.PI * 2;
                        DB.Points[PointKey].MaxAngle = NewMaxAngle;
                    }
                    else
                    {
                        for (int i = 0; i < DB.Points[PointKey].ConnectedLines.Count - 2; i++)
                        {
                            float CurrMaxAngle = DB.Points[PointKey].ConnectedLines[i + 1] - DB.Points[PointKey].ConnectedLines[i];
                            NewMaxAngle = (CurrMaxAngle > NewMaxAngle) ? CurrMaxAngle : NewMaxAngle;
                        }
                        float LastPointRads = DB.Points[PointKey].ConnectedLines[DB.Points[PointKey].ConnectedLines.Count - 1];
                        float FirstPointRadsPlusRevolution = DB.Points[PointKey].ConnectedLines[0] + (MathF.PI * 2);
                        NewMaxAngle = (LastPointRads - FirstPointRadsPlusRevolution > NewMaxAngle) ? LastPointRads - FirstPointRadsPlusRevolution : NewMaxAngle;
                    }
                    if (OldMaxAngle != NewMaxAngle)
                    {
                        DB.MaxAngularDistanceAtPoint[OldMaxAngle].Remove(PointKey);
                        if (DB.MaxAngularDistanceAtPoint.ContainsKey(NewMaxAngle))
                        {
                            DB.MaxAngularDistanceAtPoint.Add(NewMaxAngle, new());
                        }
                        DB.MaxAngularDistanceAtPoint[NewMaxAngle].Add(PointKey);

                        DB.Points[PointKey].MaxAngle = NewMaxAngle;
                    }
                }
                if (DB.MinAngularDistanceAtPoint != null)
                {
                    float OldMinAngle = DB.Points[PointKey].MinAngle.GetValueOrDefault();
                    float NewMinAngle = 100000;
                    if (DB.Points[PointKey].ConnectedLines.Count == 1)
                    {
                        NewMinAngle = MathF.PI * 2;
                        DB.Points[PointKey].MinAngle = NewMinAngle;
                    }
                    else
                    {
                        for (int i = 0; i < DB.Points[PointKey].ConnectedLines.Count - 2; i++)
                        {
                            float CurrMinAngle = DB.Points[PointKey].ConnectedLines[i + 1] - DB.Points[PointKey].ConnectedLines[i];
                            NewMinAngle = (CurrMinAngle < NewMinAngle) ? CurrMinAngle : NewMinAngle;
                        }
                        float LastPointRads = DB.Points[PointKey].ConnectedLines[DB.Points[PointKey].ConnectedLines.Count - 1];
                        float FirstPointRadsPlusRevolution = DB.Points[PointKey].ConnectedLines[0] + (MathF.PI * 2);
                        NewMinAngle = (LastPointRads - FirstPointRadsPlusRevolution < NewMinAngle) ? LastPointRads - FirstPointRadsPlusRevolution : NewMinAngle;
                    }
                    if (OldMinAngle != NewMinAngle)
                    {
                        DB.MinAngularDistanceAtPoint[OldMinAngle].Remove(PointKey);
                        if (DB.MinAngularDistanceAtPoint.ContainsKey(NewMinAngle))
                        {
                            DB.MinAngularDistanceAtPoint.Add(NewMinAngle, new());
                        }
                        DB.MinAngularDistanceAtPoint[NewMinAngle].Add(PointKey);

                        DB.Points[PointKey].MinAngle = NewMinAngle;
                    }
                }
                if (DB.LineCountAtPoint != null)
                {
                    DB.LineCountAtPoint[DB.Points[PointKey].ConnectedLines.Count - 1].Remove(PointKey);
                    if (DB.LineCountAtPoint.ContainsKey(DB.Points[PointKey].ConnectedLines.Count))
                    {
                        DB.LineCountAtPoint.Add(DB.Points[PointKey].ConnectedLines.Count, new());
                    }
                    DB.LineCountAtPoint[DB.Points[PointKey].ConnectedLines.Count].Add(PointKey);
                }
            }
            public void AddPoint(Vector2 Location, string? ID = null)
            {
                int PointKey = DB.NewUniqueElementKey();
                DB.Points.Add(PointKey, new(Location, ID));
                if(DB.MaxAngularDistanceAtPoint != null)
                {
                    DB.MaxAngularDistanceAtPoint[MathF.PI * 2].Add(PointKey);
                }
                if(DB.MinAngularDistanceAtPoint != null)
                {
                    DB.MinAngularDistanceAtPoint[MathF.PI * 2].Add(PointKey);
                }
                if(DB.LineCountAtPoint != null)
                {
                    DB.LineCountAtPoint[0].Add(PointKey);
                }
                if(DB.PointID != null && ID != null)
                {
                    if (!DB.PointID.ContainsKey(ID))
                    {
                        DB.PointID.Add(ID, new());
                    }
                    DB.PointID[ID].Add(PointKey);
                }
            }
        }
    }
}
