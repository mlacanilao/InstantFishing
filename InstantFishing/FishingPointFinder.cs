using System;

namespace InstantFishing;

public class FishingPointFinder
{
    public static void FindAndSetNearestFishingPoint()
    {
        Chara pc = EClass.pc;
        Player player = EClass.player;

        if (pc == null || player == null)
        {
            return;
        }

        HotItem? currentHotItem = player.currentHotItem;
        Thing? fishingTool = currentHotItem?.Thing;

        if (currentHotItem == null ||
            fishingTool == null ||
            fishingTool.HasElement(245, false) == false)
        {
            return;
        }

        player.TryEquipBait();

        if (EClass.debug.enable == false &&
            (player.eqBait == null || player.eqBait.isDestroyed))
        {
            return;
        }

        Point origin = pc.pos;
        int maxRadius = EClass._map?.Size ?? 0;

        if (maxRadius <= 0)
        {
            return;
        }

        for (int r = 1; r <= maxRadius; r++)
        {
            Point? bestPointOnRing = null;
            int bestDistanceSquared = int.MaxValue;

            for (int x = origin.x - r; x <= origin.x + r; x++)
            {
                for (int z = origin.z - r; z <= origin.z + r; z++)
                {
                    if (Math.Abs(value: x - origin.x) != r &&
                        Math.Abs(value: z - origin.z) != r)
                    {
                        continue;
                    }

                    Point checkPoint = new Point();
                    checkPoint.Set(_x: x, _z: z);

                    if (IsValidFishingPoint(checkPoint: checkPoint) == false)
                    {
                        continue;
                    }

                    int distanceSquared =
                        (x - origin.x) * (x - origin.x) +
                        (z - origin.z) * (z - origin.z);

                    if (distanceSquared < bestDistanceSquared)
                    {
                        bestPointOnRing = checkPoint;
                        bestDistanceSquared = distanceSquared;
                    }
                }
            }

            if (bestPointOnRing == null)
            {
                continue;
            }

            ActPlan actPlan = new ActPlan
            {
                pos = bestPointOnRing.Copy(),
                input = ActInput.RightMouse
            };

            if (currentHotItem.TrySetAct(p: actPlan) == false)
            {
                return;
            }

            actPlan._Update(target: new PointTarget { pos = bestPointOnRing.Copy() });

            var action = actPlan.GetAction();
            action?.Invoke();
            return;
        }
    }

    private static bool IsValidFishingPoint(Point checkPoint)
    {
        if (checkPoint.IsInBounds == false)
        {
            return false;
        }

        return EClass._zone?.IsUnderwater == true || checkPoint.cell.IsTopWaterAndNoSnow;
    }
}
