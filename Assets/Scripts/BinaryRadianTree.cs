using System.Collections.Generic;
using System.Numerics;

public class BinaryRadianTree<T>
{
    private List<item<T>> allItems = new List<item<T>>();
    private float radius;
    private Vector3 center;
    private BinaryRadianBranch<T> root;

    public BinaryRadianTree(List<item<T>> items, float newRadius, Vector3 newCenter)
    {
        allItems = items;
        radius = newRadius;
        center = newCenter;
    }

    public void CreateRadianTree()
    {
        root = null;

        root = new BinaryRadianBranch<T>(allItems);
    }

    class BinaryRadianBranch<T>
    {
        BinaryRadianBranch<T> leftBranch;
        BinaryRadianBranch<T> rightBranch;

        private List<item<T>> myItems = new List<item<T>>();
        private readonly Vector3 center;
        private readonly float radius;
        private readonly Plane direction;


        //TODO: Figure out how to rotate the plane along radians!
        //The plan: Split space into two parts using a plane.
        //Then do so again for each branch, aligning it along the previously splited space!
        //Image the plane looks up, then in the next branch is rotated 90 degres in one direction to split that branch space!
        public BinaryRadianBranch(List<item<T>> items) 
        {

        }
    }


    public struct item<T>
    {
        public Vector3 position;

        public T Value;

        public item(T v, float x,float y, float z)
        {
            Value = v;
            position.X = x; 
            position.Y = y; 
            position.Z = z;
        }
    }
}
