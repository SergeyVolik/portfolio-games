using Unity.Mathematics;

namespace Prototype
{
    public static class quaternionExt
    {
        public static bool IsValid(this quaternion self)
        {
            if (self.value.x != 0)
                return true;

            if (self.value.y != 0)
                return true;

            if (self.value.z != 0)
                return true;

            if (self.value.z != 0)
                return true;

            return false;
        }
    }
}