using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Camera;
using GHPC;
using UnityEngine;

namespace CustomMissionUtility
{
    internal class EditorController
    {
        public static void UnitControlHandler()
        {
            if (Cursor.lockState == CursorLockMode.None) return;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Editor.UpdateSpawnerFaction(Faction.Blue);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                Editor.UpdateSpawnerFaction(Faction.Red);
            }

            /*
            if (Input.GetKeyDown(KeyCode.C))
            {
                Editor.UpdateSpawnerFaction(Faction.Neutral);
            }
            */

            if (Input.GetMouseButtonDown(0))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("UNIT RED"))
                    {
                        Editor.SingleUnitSelected(raycastHit.collider.gameObject);
                        return;
                    }
                }

                Editor.ClearUnitSelection();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("UNIT RED"))
                    {
                        EditorTools.DeleteUnit(raycastHit.collider.gameObject);
                        return;
                    }

                    GameObject unit = EditorTools.CreateUnit(
                        raycastHit.point + new Vector3(0f, 1f, 0f),
                        new Vector3(0f, CameraManager._mainCamera.transform.eulerAngles.y, 0f));

                    Editor.SingleUnitSelected(unit);
                }
            }
        }

        public static void WaypointControlHandler()
        {
            if (Cursor.lockState == CursorLockMode.None) return;

            if (Input.GetKeyDown(KeyCode.B))
            {
                EditorWaypointGroup group = EditorTools.CreateWaypointGroup();
            }

            if (Input.GetMouseButtonDown(0))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("Waypoint"))
                    {
                        if (Editor.SELECTED_WAYPOINT_GROUPS.Count == 0 || Editor.SELECTED_WAYPOINT_GROUPS[0] != raycastHit.collider.gameObject.GetComponent<EditorWaypoint>().group)
                            Editor.WaypointGroupSelected(raycastHit.collider.gameObject.GetComponent<EditorWaypoint>().group);
                        return;
                    }
                }

                Editor.ClearWaypointGroupSelection();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("Waypoint"))
                    {
                        EditorTools.DeleteWaypoint(raycastHit.collider.gameObject);
                        return;
                    }

                    EditorWaypoint wp = EditorTools.CreateWaypoint(
                        raycastHit.point + new Vector3(0f, 1f, 0f),
                        new Vector3(0f, CameraManager._mainCamera.transform.eulerAngles.y, 0f));
                }
            }
        }
    }
}
