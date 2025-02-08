﻿using System.Numerics;
using System;
using GarageGoose.ProceduralLineNetwork;
using System.Security.Cryptography.X509Certificates;

public class ElementsManager
{
    public class ElementsDatabase
    {
        public OrderedDictionary<int, Element.Point> Points = new();
        public OrderedDictionary<int, Element.Line> Lines = new();

        //Trackers for different stuff on points, used when primarily searching for eligible points based on elimination parameters
        public SortedDictionary<float, HashSet<int>>? MaxAngularDistanceAtPoint = new();
        public SortedDictionary<float, HashSet<int>>? MinAngularDistanceAtPoint = new(Comparer<float>.Create((a, b) => b.CompareTo(a)));
        public SortedDictionary<int, HashSet<int>>? LineCountAtPoint = new();
        public SortedDictionary<int, HashSet<int>>? PointDensity = new();
        public SortedDictionary<string, HashSet<int>>? PointID = new();

        private int CurrUniqueElementKey = 0;
        public int NewUniqueElementKey()
        {
            CurrUniqueElementKey++;
            return CurrUniqueElementKey;
        }


        //Deletes and stops tracking various stuff, useful when performance is a must but parameters relying on these parameters will obvoiusly stop working when turned off (it wont crash tho)
        public ElementsDatabase StopTrackingAngularDistanceAtPoint(bool Min, bool Max)
        {
            if (Min)
            {
                MinAngularDistanceAtPoint = null;
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
            if (DB.MaxAngularDistanceAtPoint != null)
            {
                DB.MaxAngularDistanceAtPoint.Add(MathF.PI * 2, new());
            }
            if (DB.MinAngularDistanceAtPoint != null)
            {
                DB.MinAngularDistanceAtPoint.Add(MathF.PI * 2, new());
            }
            if (DB.LineCountAtPoint != null)
            {
                DB.LineCountAtPoint.Add(0, new());
            }
        }

        public void AddLine(int ConnectingPointKeyA, int ConnectingPointKeyB, float AngleAtLineA, float AngleAtLineB)
        {
            int LineKey = DB.NewUniqueElementKey();

            DB.Lines.Add(DB.NewUniqueElementKey(), new(ConnectingPointKeyA, ConnectingPointKeyB));

            //Updates the connected lines list inside the points then update the trackers
            UpdatePointAddLine(ConnectingPointKeyA, LineKey, AngleAtLineA);
            UpdatePointAddLine(ConnectingPointKeyB, LineKey, (AngleAtLineB < MathF.PI) ? AngleAtLineB + MathF.PI : AngleAtLineB - MathF.PI);
        }
        private void UpdatePointAddLine(int PointKey, int LineID, float Angle)
        {
            DB.Points[PointKey].AddLine(LineID, Angle);

            //Updates MaxAngularDistanceAtPoint tracker
            if (DB.MaxAngularDistanceAtPoint != null)
            {
                //Gets the assigned max angle of the point before finding the new one,
                //used when updating the point value since it needs to remove the old value from the database before adding the updated value
                float OldMaxAngle = DB.Points[PointKey].MaxAngle.GetValueOrDefault();

                float NewMaxAngle = 0;

                //Find the new (if any) max angle
                if (DB.Points[PointKey].ConnectedLines.Count == 1)
                {
                    //Assumes when theres only one line connected, the max angle is 2pi or 360 deg
                    NewMaxAngle = MathF.PI * 2;
                    DB.Points[PointKey].MaxAngle = NewMaxAngle;
                }
                else
                {
                    //Finds the angle between lines and updates the NewMaxAngle when its larger than NewMaxAngle
                    //Iterates between lines (except the last one, handled differently) and finds the angle between it and the line ahead
                    for (int i = 0; i < DB.Points[PointKey].ConnectedLines.Count - 2; i++)
                    {
                        float CurrMaxAngle = DB.Points[PointKey].ConnectedLines[i + 1] - DB.Points[PointKey].ConnectedLines[i];
                        NewMaxAngle = (CurrMaxAngle > NewMaxAngle) ? CurrMaxAngle : NewMaxAngle;
                    }

                    //Does the same thing but the procedure is different
                    //It compares the first and last line vaule and assumes that the first line is one revolusion ahead
                    float LastPointRads = DB.Points[PointKey].ConnectedLines[DB.Points[PointKey].ConnectedLines.Count - 1];
                    float FirstPointRadsPlusRevolution = DB.Points[PointKey].ConnectedLines[0] + (MathF.PI * 2);
                    NewMaxAngle = (LastPointRads - FirstPointRadsPlusRevolution > NewMaxAngle) ? LastPointRads - FirstPointRadsPlusRevolution : NewMaxAngle;
                }

                //Updates the value to the database
                if (OldMaxAngle != NewMaxAngle)
                {
                    //Removes the outdated info on the database
                    DB.MaxAngularDistanceAtPoint[OldMaxAngle].Remove(PointKey);

                    //Adds the updated info to the database
                    //Checks if the particular value already exists in the database and if not, it adds it
                    if (DB.MaxAngularDistanceAtPoint.ContainsKey(NewMaxAngle))
                    {
                        DB.MaxAngularDistanceAtPoint.Add(NewMaxAngle, new());
                    }

                    //Adds the PointKey to that value
                    DB.MaxAngularDistanceAtPoint[NewMaxAngle].Add(PointKey);

                    //Updates the stored info on the point
                    DB.Points[PointKey].MaxAngle = NewMaxAngle;
                }
            }

            //Updates MinAngularDistanceAtPoint tracker
            if (DB.MinAngularDistanceAtPoint != null)
            {
                //Gets the assigned min angle of the point before finding the new one,
                //used when updating the point value since it needs to remove the old value from the database before adding the updated value
                float OldMinAngle = DB.Points[PointKey].MinAngle.GetValueOrDefault();

                //Worst case scenario for a min angle
                float NewMinAngle = MathF.PI * 2;

                //Find the new (if any) min angle
                if (DB.Points[PointKey].ConnectedLines.Count == 1)
                {
                    //Assumes when theres only one line connected, the min angle is 2pi or 360 deg
                    NewMinAngle = MathF.PI * 2;
                    DB.Points[PointKey].MinAngle = NewMinAngle;
                }
                else
                {
                    //Finds the angle between lines and updates the NewMinAngle when its smaller than NewMinAngle
                    //Iterates between lines (except the last one, handled differently) and finds the angle between it and the line ahead
                    for (int i = 0; i < DB.Points[PointKey].ConnectedLines.Count - 2; i++)
                    {
                        float CurrMinAngle = DB.Points[PointKey].ConnectedLines[i + 1] - DB.Points[PointKey].ConnectedLines[i];
                        NewMinAngle = (CurrMinAngle < NewMinAngle) ? CurrMinAngle : NewMinAngle;
                    }

                    //Does the same thing but the procedure is different
                    //It compares the first and last line vaule and assumes that the first line is one revolusion ahead
                    float LastPointRads = DB.Points[PointKey].ConnectedLines[DB.Points[PointKey].ConnectedLines.Count - 1];
                    float FirstPointRadsPlusRevolution = DB.Points[PointKey].ConnectedLines[0] + (MathF.PI * 2);
                    NewMinAngle = (LastPointRads - FirstPointRadsPlusRevolution < NewMinAngle) ? LastPointRads - FirstPointRadsPlusRevolution : NewMinAngle;
                }

                //Updates the value to the database
                if (OldMinAngle != NewMinAngle)
                {
                    //Removes the outdated info on the database
                    DB.MinAngularDistanceAtPoint[OldMinAngle].Remove(PointKey);

                    //Adds the updated info to the database
                    //Checks if the particular value already exists in the database and if not, it adds it
                    if (DB.MinAngularDistanceAtPoint.ContainsKey(NewMinAngle))
                    {
                        DB.MinAngularDistanceAtPoint.Add(NewMinAngle, new());
                    }

                    //Adds the PointKey to that value
                    DB.MinAngularDistanceAtPoint[NewMinAngle].Add(PointKey);

                    //Updates the stored info on the point
                    DB.Points[PointKey].MinAngle = NewMinAngle;
                }
            }

            //Updates LineCountAtPoint tracker
            //Assumes that the new line is added before before reaching this
            if (DB.LineCountAtPoint != null)
            {
                //Removes outdated information to the database
                DB.LineCountAtPoint[DB.Points[PointKey].ConnectedLines.Count - 1].Remove(PointKey);

                //Checks if the particular value already exists in the database and if not, it adds it
                if (DB.LineCountAtPoint.ContainsKey(DB.Points[PointKey].ConnectedLines.Count))
                {
                    DB.LineCountAtPoint.Add(DB.Points[PointKey].ConnectedLines.Count, new());
                }

                //Adds the PointKey to that value
                DB.LineCountAtPoint[DB.Points[PointKey].ConnectedLines.Count].Add(PointKey);
            }
        }
        public void AddPoint(Vector2 Location, string[]? ID = null)
        {
            //Assignes the unique key for the new element (point), used for referencing this line
            int PointKey = DB.NewUniqueElementKey();

            DB.Points.Add(PointKey, new(Location, ID));

            //Add relevant values to any active trackers
            if (DB.MaxAngularDistanceAtPoint != null)
            {
                DB.MaxAngularDistanceAtPoint[MathF.PI * 2].Add(PointKey);
            }
            if (DB.MinAngularDistanceAtPoint != null)
            {
                DB.MinAngularDistanceAtPoint[MathF.PI * 2].Add(PointKey);
            }
            if (DB.LineCountAtPoint != null)
            {
                DB.LineCountAtPoint[0].Add(PointKey);
            }
            if (DB.PointID != null && ID != null)
            {
                foreach (string IDElement in ID)
                {
                    if (!DB.PointID.ContainsKey(IDElement))
                    {
                        DB.PointID.Add(IDElement, new());
                    }
                    DB.PointID[IDElement].Add(PointKey);
                }
            }
        }
        
    }
}
