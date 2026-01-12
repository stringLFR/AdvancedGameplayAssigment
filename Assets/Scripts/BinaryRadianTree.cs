using System.Collections.Generic;
using System.Linq;
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

        root.FindClosesItems(amount, position,ref items);

        return items;
    }

    public void CreateRadianTree(int depth, List<BRT_item<T>> items = null)
    {
        root = null;

        if (items != null) allItems = items;

        if (allItems == null) return;

        allItems.Sort((x,y) => Vector3.Distance(x.position,center).CompareTo(Vector3.Distance(y.position, center))); 

        root = new BinaryRadianBranch<T>(allItems, center, radius, radius, false,true, depth, 0);
    }

    class BinaryRadianBranch<T>
    {
        BinaryRadianBranch<T> leftBranch;
        BinaryRadianBranch<T> rightBranch;

        private readonly List<BRT_item<T>> myItems = new List<BRT_item<T>>();
        private readonly Vector3 myCenter;
        private readonly float myRadius;
        private readonly Plane direction;

        public void FindClosesItems(int amount, Vector3 position, ref List<BRT_item<T>> items)
        {
            if (leftBranch != null) if (Plane.DotNormal(direction, position) > 0) leftBranch.FindClosesItems(amount, position, ref items);

            if (rightBranch != null) if (Plane.DotNormal(direction, position) < 0) rightBranch.FindClosesItems(amount, position, ref items);

            if (items.Count >= amount) return;

            for (int i = 0; i < myItems.Count; i++)
            {
                BRT_item<T> bestItem = myItems.LastOrDefault();

                foreach (BRT_item<T> j in myItems)
                {
                    if (Vector3.Distance(position, j.position) <= Vector3.Distance(position, bestItem.position))
                    {
                        bestItem = j;
                    }
                }

                if (items.Contains(bestItem)) continue;

                items.Add(bestItem);
            }
        }

        public BinaryRadianBranch(List<BRT_item<T>> items, Vector3 center, float baseRadius, float currentRadius, bool upAxis,bool isRight, int maxDepth, int currentDepth) 
        {
            myCenter = center;
            myRadius = baseRadius;
            Vector3 p1, p2, p3;
            float newRadiusModifier = currentDepth > 1 ? currentRadius / 2 : currentRadius;

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

            if (maxDepth <= currentDepth) return;

            List<BRT_item<T>> leftItems = new List<BRT_item<T>>();
            List<BRT_item<T>> rightItems = new List<BRT_item<T>>();

            foreach (BRT_item<T> i in items)
            {
                float test = Vector3.Distance(i.position, center);

                if (test > baseRadius) continue;

                myItems.Add(i);

                float dotProduct = Plane.DotNormal(direction, i.position);

                if (dotProduct > 0) leftItems.Add(i);
                else rightItems.Add(i);
            }

            bool axis = upAxis == true ? false : true;

            if (leftItems.Count > 0) leftBranch = new BinaryRadianBranch<T>(leftItems, myCenter, myRadius, newRadiusModifier, axis,false, maxDepth, currentDepth + 1);

            if (rightItems.Count > 0) rightBranch = new BinaryRadianBranch<T>(rightItems, myCenter, myRadius, newRadiusModifier, axis, true, maxDepth, currentDepth + 1);
        }
    }
}
