using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using TMPro;
using UnityEngine.EventSystems;

namespace CustomMissionUtility
{
	internal class Draggable : MonoBehaviour, IDragHandler
	{
		public Transform parent;

		public void OnDrag(PointerEventData eventData)
		{
			parent.position = eventData.position;
		}
	}
}