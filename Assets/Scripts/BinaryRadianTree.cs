using System.Collections.Generic;
using System.Numerics;

public struct BRT_item<T>
{
    public Vector3 position;

    public T Value;

    public BRT_item(T v, float x, float y, float z)
    {
        Value = v;
        position.X = x;
        position.Y = y;
        position.Z = z;
    }
}

public class BinaryRadianTree<T>
{
    private List<BRT_item<T>> allItems = new List<BRT_item<T>>();
    private float radius;
    private Vector3 center;
    private BinaryRadianBranch<T> root;

    public BinaryRadianTree(float newRadius, Vector3 newCenter)
    {
        radius = newRadius;
        center = newCenter;
    }

    public List<BRT_item<T>> FindClosesItems(int amount, Vector3 position)
    {
        List<BRT_item<T>> items = new List<BRT_item<T>>();

        root.FindClosesItems(amount, position,ref items, Plane.DotNormal(root.direction, position));

        return items;
    }

    public void CreateRadianTree(int depth, List<BRT_item<T>> items = null)
    {
        root = null;

        if (items != null) allItems = items;

        if (allItems == null) return;

        allItems.Sort((x,y) => Vector3.Distance(x.position,center).CompareTo(Vector3.Distance(y.position, center))); 

        root = new BinaryRadianBranch<T>(allItems, center, radius, radius, true,true, depth, 0);
    }

    class BinaryRadianBranch<T>
    {
        BinaryRadianBranch<T> leftBranch;
        BinaryRadianBranch<T> rightBranch;

        public readonly List<BRT_item<T>> myItems = new List<BRT_item<T>>();
        public readonly Vector3 myCenter;
        public readonly float myRadius;
        public readonly Plane direction;
        public readonly bool upAxis;
        public readonly bool isRight;
        public readonly int MaxDepth;

        //TODO: It's not consistent!!!!!!!!!!!!!!
        public void FindClosesItems(int amount, Vector3 position, ref List<BRT_item<T>> items, float lastDotProduct)
        {
            if (myItems.Count == 0) return;

            float dot = Plane.DotNormal(direction, position);

            if (isRight)
            {
                if (lastDotProduct > 0)
                {
                    rightBranch.FindClosesItems(amount, position, ref items, dot);
                    CheckItems(amount, ref items, position);
                    leftBranch.FindClosesItems(amount, position, ref items, dot);
                }
                else
                {
                    leftBranch.FindClosesItems(amount, position, ref items, dot);
                    CheckItems(amount, ref items, position);
                    rightBranch.FindClosesItems(amount, position, ref items, dot);
                }
            }
            else
            {
                if (lastDotProduct > 0)
                {
                    leftBranch.FindClosesItems(amount, position, ref items, dot);
                    CheckItems(amount, ref items,position);
                    rightBranch.FindClosesItems(amount, position, ref items, dot);

                }
                else
                {
                    rightBranch.FindClosesItems(amount, position, ref items, dot);
                    CheckItems(amount, ref items, position);
                    leftBranch.FindClosesItems(amount, position, ref items, dot);
                }
            }

            return;

            void CheckItems(int amount, ref List<BRT_item<T>> items, Vector3 position)
            {
                if (items.Count >= amount) return;

                for (int i = 0; i < amount; i++)
                {
                    BRT_item<T> best = myItems[0];

                    for (int j = 0; j < myItems.Count; j++)
                    {
                        if (items.Contains(myItems[j])) continue;

                        if (Vector3.Distance(position, best.position) > Vector3.Distance(position, myItems[j].position))
                        {
                            best = myItems[j];
                        }
                    }

                    if (items.Contains(best)) continue;

                    items.Add(best);
                }
            }
        }

        //TODO: Figure out if the creation process causes search inconsistens!!!!
        public BinaryRadianBranch(List<BRT_item<T>> items, Vector3 center, float baseRadius, float currentRadius, bool Axis,bool Right, int maxDepth, int currentDepth) 
        {
            if (items.Count == 0) return;

            myCenter = center;
            myRadius = baseRadius;
            MaxDepth = maxDepth;
            Vector3 p1, p2, p3;
            float newRadiusModifier = currentDepth > 0 ? currentRadius / 2 : currentRadius;

            upAxis = Axis;
            isRight = Right;

            if (upAxis == true)
            {
                if (isRight)
                {
                    p1 = myCenter + new Vector3(newRadiusModifier, 0, 0);
                    p2 = myCenter + new Vector3(0, myRadius - newRadiusModifier, 0);
                    p3 = myCenter + new Vector3(0, 0, newRadiusModifier);
                }
                else
                {
                    p1 = myCenter + new Vector3(-newRadiusModifier, 0, 0);
                    p2 = myCenter + new Vector3(0, myRadius - newRadiusModifier, 0);
                    p3 = myCenter + new Vector3(0, 0, -newRadiusModifier);
                }
            }
            else
            {
                if (isRight)
                {
                    p1 = myCenter + new Vector3(newRadiusModifier, 0, 0);
                    p2 = myCenter + new Vector3(0, newRadiusModifier, 0);
                    p3 = myCenter + new Vector3(0, 0, myRadius - newRadiusModifier);
                }
                else
                {
                    p1 = myCenter + new Vector3(-newRadiusModifier, 0, 0);
                    p2 = myCenter + new Vector3(0, -newRadiusModifier, 0);
                    p3 = myCenter + new Vector3(0, 0, myRadius - newRadiusModifier);
                }
                
            }

            direction = Plane.CreateFromVertices(p1, p2, p3);


            UnityEngine.Debug.DrawRay(new UnityEngine.Vector3(center.X, center.Y, center.Z), new UnityEngine.Vector3(direction.Normal.X, direction.Normal.Y, direction.Normal.Z), UnityEngine.Color.red, 1000f);
            UnityEngine.Debug.DrawRay(new UnityEngine.Vector3(center.X, center.Y, center.Z), new UnityEngine.Vector3(-direction.Normal.X, -direction.Normal.Y, -direction.Normal.Z), UnityEngine.Color.green, 1000f);


            if (maxDepth <= currentDepth) return;

            List<BRT_item<T>> leftItems = new List<BRT_item<T>>();
            List<BRT_item<T>> rightItems = new List<BRT_item<T>>();

            foreach (BRT_item<T> i in items)
            {
                float test = Vector3.Distance(i.position, center);

                if (test > baseRadius) continue;

                myItems.Add(i);

                float dotProduct = Plane.DotNormal(direction, i.position);


                if (isRight)
                {
                    if (dotProduct > 0) rightItems.Add(i);
                    else leftItems.Add(i);
                }
                else
                {
                    if (dotProduct > 0) leftItems.Add(i);
                    else rightItems.Add(i);
                }
            }

            bool axis = upAxis == true ? false : true;

            leftBranch = new BinaryRadianBranch<T>(leftItems, myCenter, myRadius, newRadiusModifier, axis,false, maxDepth, currentDepth + 1);

            rightBranch = new BinaryRadianBranch<T>(rightItems, myCenter, myRadius, newRadiusModifier, axis, true, maxDepth, currentDepth + 1);
        }
    }
}
