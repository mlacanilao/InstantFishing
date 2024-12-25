namespace InstantFishing
{
    public class FishingPointFinder
    {
        public static void FindAndSetNearestFishingPoint()
        {
            Point nearestValidPoint = null;
            int shortestDistance = int.MaxValue;

            // Iterate over all points in map bounds
            ELayer._map.bounds.ForeachPoint(action: delegate(Point point)
            {
                // InstantFishing.Log($"Checking point: {point}");

                // Check if the point is valid for fishing
                if (!point.IsInBounds)
                {
                    // InstantFishing.Log($"Point {point} is out of bounds. Skipping.");
                    return; // Skip invalid points
                }

                if (!point.cell.IsTopWaterAndNoSnow)
                {
                    // InstantFishing.Log($"Point {point} is not top water or has snow. Skipping.");
                    return; // Skip points not suitable for fishing
                }

                // Calculate the distance from the player's position to this point
                int distance = EClass.pc.pos.Distance(p: point);
                // InstantFishing.Log($"Distance to point {point}: {distance}");

                // Check if this is the nearest valid point so far
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestValidPoint = point.Copy(); // Store the nearest point
                    InstantFishing.Log(payload: $"New nearest point found: {point}");
                }
            });

            if (nearestValidPoint != null)
            {
                // InstantFishing.Log($"Nearest valid point found: {nearestValidPoint} with distance: {shortestDistance}");

                // Create an ActPlan for the nearest valid point
                ActPlan actPlan = new ActPlan();
                actPlan.pos = nearestValidPoint.Copy();
                // InstantFishing.Log($"Created ActPlan for nearest point: {nearestValidPoint}");

                // Try to set the act for the nearest point
                if (EClass.player.currentHotItem?.TrySetAct(p: actPlan) == true)
                {
                    // InstantFishing.Log($"Successfully set tool act for nearest point: {nearestValidPoint}");

                    // Simulate right-click on the nearest point
                    actPlan.input = ActInput.RightMouse; // Set input type to right-click
                    actPlan._Update(target: new PointTarget { pos = nearestValidPoint.Copy() });
                    // InstantFishing.Log($"Simulated right-click on nearest point: {nearestValidPoint}");

                    // Execute the action
                    var action = actPlan.GetAction();
                    if (action != null && action())
                    {
                        // InstantFishing.Log($"Action executed successfully for nearest point: {nearestValidPoint}");
                    }
                    else
                    {
                        // InstantFishing.Log($"Action execution failed for nearest point: {nearestValidPoint}");
                    }
                }
            }
            else
            {
                InstantFishing.Log(payload: $"No valid fishing point found.");
            }
        }
    }
}