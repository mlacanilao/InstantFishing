using System;

namespace InstantFishing
{
    public class FishingPointFinder
    {
        public static void FindAndSetNearestFishingPoint()
        {
            Point newFishingPoint = null;
            Point origin = EClass.pc.pos;

            int maxRadius = 20; // Optional: limit for performance

            for (int r = 1; r <= maxRadius; r++)
            {
                for (int x = origin.x - r; x <= origin.x + r; x++)
                {
                    for (int z = origin.z - r; z <= origin.z + r; z++)
                    {
                        // Skip inner area to only hit the current radius ring
                        if (Math.Abs(value: x - origin.x) != r && Math.Abs(value: z - origin.z) != r)
                            continue;

                        Point checkPoint = new Point();
                        checkPoint.Set(_x: x, _z: z);

                        if (!checkPoint.IsInBounds)
                            continue;

                        if (!checkPoint.cell.IsTopWaterAndNoSnow)
                            continue;

                        newFishingPoint = checkPoint;
                        break;
                    }

                    if (newFishingPoint != null) break;
                }

                if (newFishingPoint != null) break;
            }

            if (newFishingPoint != null)
            {
                ActPlan actPlan = new ActPlan
                {
                    pos = newFishingPoint.Copy(),
                    input = ActInput.RightMouse
                };

                if (EClass.player.currentHotItem?.TrySetAct(p: actPlan) == true)
                {
                    actPlan._Update(target: new PointTarget { pos = newFishingPoint.Copy() });

                    var action = actPlan.GetAction();
                    if (action != null && action())
                    {
                        InstantFishing.Log(payload: "Successfully set fishing action.");
                    }
                }
            }
            else
            {
                InstantFishing.Log(payload: "No valid fishing point found within radius.");
            }
        }
    }
}