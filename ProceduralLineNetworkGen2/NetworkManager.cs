using System.Numerics;
using System.Runtime.InteropServices;
using GarageGoose.ProceduralLineNetwork.Elements;

public class ElementsDatabase
{
    public Dictionary<uint, Point> Points = new();
    public List<uint>? PointKeysList = new();

    public Dictionary<uint, Line> Lines = new();

    //Trackers for different stuff on points, used when primarily searching for eligible points based on elimination parameters
    public struct ValuedPointKeyHashSet
    {
        public float Val;
        public ValuedPointKeyHashSet(float val)
        {
            Val = val;
        }
        public HashSet<uint> PointKey = new();
    }
    public SortedSet<ValuedPointKeyHashSet>? MaxAngularDistanceAtPoint = new(Comparer<ValuedPointKeyHashSet>.Create((a, b) => a.Val.CompareTo(b.Val)));
    public SortedSet<ValuedPointKeyHashSet>? MinAngularDistanceAtPoint = new(Comparer<ValuedPointKeyHashSet>.Create((a, b) => b.Val.CompareTo(a.Val)));
    public SortedSet<ValuedPointKeyHashSet>? LineCountAtPoint = new(Comparer<ValuedPointKeyHashSet>.Create((a, b) => a.Val.CompareTo(b.Val)));
    public bool MaxADPGetRef(float Angle, out ValuedPointKeyHashSet Ref)
    {
        if (MaxAngularDistanceAtPoint != null && MaxAngularDistanceAtPoint.TryGetValue(new(Angle), out Ref))
        {
            return true;
        }
        Ref = new(0);
        return false;
    }
    public bool MinADPGetRef(float Angle, out ValuedPointKeyHashSet Ref)
    {
        if (MinAngularDistanceAtPoint != null && MinAngularDistanceAtPoint.TryGetValue(new(Angle), out Ref))
        {
            return true;
        }
        Ref = new(0);
        return false;
    }
    public bool LineCountGetRef(int Count, out ValuedPointKeyHashSet Ref)
    {
        if (LineCountAtPoint != null && LineCountAtPoint.TryGetValue(new(Count), out Ref))
        {
            return true;
        }
        Ref = new(0);
        return false;
    }
    public Dictionary<string, HashSet<uint>>? PointID = new();


    
    private uint CurrUniqueElementKey = 0;
    public uint NewUniqueElementKey()
    {
        CurrUniqueElementKey++;
        return CurrUniqueElementKey;
    }
}

public class ElementsDatabaseHandler
{
    private ElementsDatabase DB;
    public ElementsDatabaseHandler(ref ElementsDatabase DBRef)
    {
        DB = DBRef;
    }

    private void UpdatePointLines(uint PointKey, uint LineKey, bool AddLine, float Angle = -1)
    {
        //Use a single reference for the entire operation rather than calling dict each time access is needed
        Point CurrPointRef = DB.Points[PointKey];

        if (AddLine)
        {
            CurrPointRef.AddLine(LineKey, Angle);
        }
        else
        {
            CurrPointRef.RemoveLine(LineKey);
        }


        //Updates MaxAngularDistanceAtPoint tracker
        if (DB.MaxAngularDistanceAtPoint != null)
        {
            //Gets the assigned max angle of the point before finding the new one,
            //used when updating the point value since it needs to remove the old value from the database before adding the updated value
            float OldMaxAngle = CurrPointRef.MaxAngle.GetValueOrDefault();

            float NewMaxAngle = 0;

            //Find the new (if any) max angle
            if (CurrPointRef.ConnectedLines.Count == 1)
            {
                //Assumes when theres only one line connected, the max angle is 2pi or 360 deg
                NewMaxAngle = MathF.PI * 2;
                CurrPointRef.MaxAngle = NewMaxAngle;
            }
            else
            {
                //Finds the angle between lines and updates the NewMaxAngle when its larger than NewMaxAngle
                //Iterates between lines (except the last one, handled differently) and finds the angle between it and the line ahead
                for (int i = 0; i < CurrPointRef.ConnectedLines.Count - 2; i++)
                {
                    float CurrMaxAngle = CurrPointRef.ConnectedLines[i + 1] - CurrPointRef.ConnectedLines[i];
                    NewMaxAngle = (CurrMaxAngle > NewMaxAngle) ? CurrMaxAngle : NewMaxAngle;
                }

                //Does the same thing but the procedure is different
                //It compares the first and last line vaule and assumes that the first line is one revolusion ahead
                float LastPointRads = CurrPointRef.ConnectedLines[CurrPointRef.ConnectedLines.Count - 1];
                float FirstPointRadsPlusRevolution = CurrPointRef.ConnectedLines[0] + (MathF.PI * 2);
                float FirstAndLastPointGap = FirstPointRadsPlusRevolution - LastPointRads;
                NewMaxAngle = (FirstAndLastPointGap > NewMaxAngle) ? FirstAndLastPointGap : NewMaxAngle;
            }

            //Updates the value to the database
            if (OldMaxAngle != NewMaxAngle)
            {
                //Removes the outdated info on the database
                if (DB.MaxADPGetRef(OldMaxAngle, out var MADP_RefOld))
                {
                    MADP_RefOld.PointKey.Remove(PointKey);
                }

                //Adds the updated info to the database
                //Checks if the particular value already exists in the database and if not, it adds it
                if (DB.MaxADPGetRef(NewMaxAngle, out var MADP_RefNew))
                {
                    MADP_RefNew.PointKey.Add(PointKey);
                }
                else
                {
                    ElementsDatabase.ValuedPointKeyHashSet New = new(NewMaxAngle);
                    New.PointKey.Add(PointKey);
                    DB.MaxAngularDistanceAtPoint.Add(New);
                }

                //Updates the stored info on the point
                CurrPointRef.MaxAngle = NewMaxAngle;
            }
        }

        //Updates MinAngularDistanceAtPoint tracker
        if (DB.MinAngularDistanceAtPoint != null)
        {
            //Gets the assigned min angle of the point before finding the new one,
            //used when updating the point value since it needs to remove the old value from the database before adding the updated value
            float OldMinAngle = CurrPointRef.MinAngle.GetValueOrDefault();

            //Worst case scenario for a min angle
            float NewMinAngle = MathF.PI * 2;

            //Find the new (if any) min angle
            if (CurrPointRef.ConnectedLines.Count == 1)
            {
                //Assumes when theres only one line connected, the min angle is 2pi or 360 deg
                NewMinAngle = MathF.PI * 2;
                CurrPointRef.MinAngle = NewMinAngle;
            }
            else
            {
                //Finds the angle between lines and updates the NewMinAngle when its smaller than NewMinAngle
                //Iterates between lines (except the last one, handled differently) and finds the angle between it and the line ahead
                for (int i = 0; i < CurrPointRef.ConnectedLines.Count - 2; i++)
                {
                    float CurrMinAngle = CurrPointRef.ConnectedLines[i + 1] - CurrPointRef.ConnectedLines[i];
                    NewMinAngle = (CurrMinAngle < NewMinAngle) ? CurrMinAngle : NewMinAngle;
                }

                //Does the same thing but the procedure is different
                //It compares the first and last line vaule and assumes that the first line is one revolusion ahead
                float LastPointRads = DB.Points[PointKey].ConnectedLines[CurrPointRef.ConnectedLines.Count - 1];
                float FirstPointRadsPlusRevolution = CurrPointRef.ConnectedLines[0] + (MathF.PI * 2);
                float FirstAndLastPointGap = FirstPointRadsPlusRevolution - LastPointRads;
                NewMinAngle = (FirstAndLastPointGap < NewMinAngle) ? FirstAndLastPointGap : NewMinAngle;
            }

            //Updates the value to the database
            if (OldMinAngle != NewMinAngle)
            {
                //Removes the outdated info on the database
                if (DB.MinADPGetRef(OldMinAngle, out var MADP_RefOld))
                {
                    MADP_RefOld.PointKey.Remove(PointKey);
                }

                //Adds the updated info to the database
                //Checks if the particular value already exists in the database and if not, it adds it
                if (DB.MinADPGetRef(NewMinAngle, out var MADP_RefNew))
                {
                    MADP_RefNew.PointKey.Add(PointKey);
                }
                else
                {
                    ElementsDatabase.ValuedPointKeyHashSet New = new(NewMinAngle);
                    New.PointKey.Add(PointKey);
                    DB.MinAngularDistanceAtPoint.Add(New);
                }

                //Updates the stored info on the point
                CurrPointRef.MinAngle = NewMinAngle;
            }
        }

        //Updates LineCountAtPoint tracker
        if (DB.LineCountAtPoint != null)
        {
            //Removes outdated information to the database
            int OldLineCount = AddLine ? -1 : 1;
            if (DB.LineCountGetRef(CurrPointRef.ConnectedLines.Count + OldLineCount, out var LC_Old))
            {
                LC_Old.PointKey.Remove(PointKey);
            }

            //Checks if the particular value already exists in the database and if not, it adds it
            if (DB.LineCountGetRef(CurrPointRef.ConnectedLines.Count, out var LC_New))
            {
                LC_New.PointKey.Add(PointKey);
            }
            else
            {
                ElementsDatabase.ValuedPointKeyHashSet New = new(CurrPointRef.ConnectedLines.Count);
                New.PointKey.Add(PointKey);
                DB.LineCountAtPoint.Add(New);
            }
        }
    }
    public void AddLine(uint ConnectingPointKeyA, uint ConnectingPointKeyB, float AngleAtLineA, float AngleAtLineB)
    {
        uint LineKey = DB.NewUniqueElementKey();

        DB.Lines.Add(LineKey, new(ConnectingPointKeyA, ConnectingPointKeyB));

        //Updates the connected lines list inside the points then update the trackers
        UpdatePointLines(ConnectingPointKeyA, LineKey, true, AngleAtLineA);
        float OppositeAngle = (AngleAtLineB < MathF.PI) ? AngleAtLineB + MathF.PI : AngleAtLineB - MathF.PI;
        UpdatePointLines(ConnectingPointKeyB, LineKey, true, OppositeAngle);
    }
    public void AddPoint(Vector2 Location, string[]? ID = null)
    {
        //Assignes the unique key for the new element (point), used for referencing this line
        uint PointKey = DB.NewUniqueElementKey();

        DB.Points.Add(PointKey, new(Location, ID));

        //Add relevant values to any active trackers
        if (DB.MaxAngularDistanceAtPoint != null && DB.MaxADPGetRef(MathF.PI * 2, out var MaxADP))
        {
            MaxADP.PointKey.Add(PointKey);
        }
        if (DB.MinAngularDistanceAtPoint != null && DB.MinADPGetRef(MathF.PI * 2, out var MinADP))
        {
            MinADP.PointKey.Add(PointKey);
        }
        if (DB.LineCountAtPoint != null && DB.LineCountGetRef(0, out var LC))
        {
            LC.PointKey.Add(PointKey);
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
    public void DeleteLine(uint Key)
    {
        Line CurrLine = DB.Lines[Key];
        UpdatePointLines(CurrLine.PointKey1, Key, false);
        UpdatePointLines(CurrLine.PointKey2, Key, false);
        DB.Lines.Remove(Key);
    }
    public void DeletePoint(uint Key)
    {

    }
    public HashSet<uint> GetAllMaxAngularDistanceAtPointInRange(float Min, float Max)
    {
        if (DB.MaxAngularDistanceAtPoint == null)
        {
            return new();
        }
        HashSet<uint> PointKeys = new();
        foreach (ElementsDatabase.ValuedPointKeyHashSet HashSets in DB.MaxAngularDistanceAtPoint.GetViewBetween(new(Min), new(Max)))
        {
            PointKeys.UnionWith(HashSets.PointKey);
        }
        return PointKeys;
    }
    public HashSet<uint> GetAllMinAngularDistanceAtPointInRange(float Min, float Max)
    {
        if (DB.MinAngularDistanceAtPoint == null)
        {
            return new();
        }
        HashSet<uint> PointKeys = new();
        foreach (ElementsDatabase.ValuedPointKeyHashSet HashSets in DB.MinAngularDistanceAtPoint.GetViewBetween(new(Min), new(Max)))
        {
            PointKeys.UnionWith(HashSets.PointKey);
        }
        return PointKeys;
    }
    public HashSet<uint> GetAllLineCountAtPointInRange(int Min, int Max)
    {
        if(DB.LineCountAtPoint == null)
        {
            return new();
        }
        HashSet<uint> PointKeys = new();
        foreach(ElementsDatabase.ValuedPointKeyHashSet HashSets in DB.LineCountAtPoint.GetViewBetween(new(Min), new(Max)))
        {
            PointKeys.UnionWith(HashSets.PointKey);
        }
        return PointKeys;
    }
    public HashSet<uint> GetAllPointIDAtID(string ID)
    {
        if (DB.PointID == null)
        {
            return new();
        }
        return DB.PointID[ID];
    }
    public HashSet<uint> GetPointsInOrderOfAdditionRange(int Min, int Max)
    {
        HashSet<uint> PointKeys = new();
        if (DB.PointKeysList != null)
        {
            for (int i = Min; i <= Max; i++)
            {
                PointKeys.Add(DB.PointKeysList[i]);
            }
        }
        return PointKeys;
    }
}

public class NetworkCompute
{
    private ElementsDatabase DB;
    public NetworkCompute(ref ElementsDatabase DBRef)
    {
        DB = DBRef;
    }
}
