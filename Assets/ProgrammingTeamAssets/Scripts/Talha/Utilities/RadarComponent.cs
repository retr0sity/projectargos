using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.Components
{
	[AddComponentMenu("Radar Arrows/New UI Radar Arrows")]
	public class RadarComponent : MonoBehaviour
	{

		public Camera cam;
		public Sprite[] arrowSprites; //the texture for your arrow
		public string[] tagsToFind; //the tag that the script looks for
		public float size = 30; //dimension (width and height should be the same)
		public bool hoverOnScreen = true; //whether arrows should appear over the object onscreen
		public bool projectToEdge = true; //whether arrows should show for offscreen objects
		public float distanceAbove = 1; //if hoverOnScreen, the distance above the arrow should hover
		public float blindSpot = .5f; //Size of area behind screen where objects are "invisible" to Radar Arrows
		public float hoverAngle = 270;
		public bool projectFromHoverPoint = true; //if false, will project from center of the target

		private float xCenter; //center of screen horizontally
		private float yCenter; //center of screen vertically
		private float halfSize; //half of the dimension
		private float screenSlope; //slope diagonally across the screen
		private bool errorless = false; //whether the program will execute
		private Canvas canvas;
		private List<Arrow>[] arrows; //References to all the arrows currently in the scene

		//Setters (needed for UI
		public void SetHoverOnScreen(bool hover) { hoverOnScreen = hover; }
		public void SetSize(float newSize) { size = newSize; }
		public void SetDistanceAbove(float newDistanceAbove) { distanceAbove = newDistanceAbove; }
		public void SetBlindSpot(float newBlindSpot) { blindSpot = newBlindSpot; }
		public void SetHoverAngle(float newHoverAngle) { hoverAngle = newHoverAngle; }
		public void SetProjectFromHoverPoint(bool fromHover) { projectFromHoverPoint = fromHover; }
		public void SetProjectToEdge(bool project) { projectToEdge = project; }

		void Start()
		{
			if (transform.parent != null)
			{
				canvas = transform.parent.GetComponent<Canvas>();
			}
			if (cam == null)
			{
				Debug.Log("You must attach a camera reference to RadarArrows");
			}
			else if (canvas == null)
			{
				Debug.Log("Radar Arrows must be the child of a canvas.");
			}
			else if (arrowSprites.Length <= 0)
			{
				Debug.Log("Radar Arrows requires at least one sprite.");
			}
			else if (tagsToFind.Length <= 0)
			{
				Debug.Log("Radar Arrows requires at least one tag to find.");
			}
			else if (arrowSprites.Length == tagsToFind.Length)
			{
				arrows = new List<Arrow>[arrowSprites.Length];
				for (int i = 0; i < arrowSprites.Length; i++)
				{
					arrows[i] = new List<Arrow>();
				}
				errorless = true;
			}
			else
			{
				Debug.Log("There should be exactly one sprite for every tag on the RadarArrows script");
			}
		}

		void LateUpdate()
		{
			if (errorless)
			{
				for (int i = 0; i < arrows.Length; i++)
				{
					FixArrowList(i);
				}
			}
		}

		/// <summary>
		/// Destroys arrows for destroyed objects, instantiates arrows for recently instantiate
		/// objects, and updates the location of all arrows.
		/// </summary>
		/// <param name="arrowType">Index in the arrowSprite/tagsToFind array</param>
		void FixArrowList(int arrowType)
		{
			List<Arrow> arrowList = arrows[arrowType];
			GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tagsToFind[arrowType]).Where(x => x.gameObject.activeInHierarchy).ToArray();
			for (int i = 0; i < taggedObjects.Length || i < arrowList.Count; i++)
			{
				if (i >= taggedObjects.Length || (i < arrowList.Count && arrowList[i].tar == null))
				{
					GameObject.Destroy(arrowList[i].sprite);
					arrowList.RemoveAt(i);
					i--;
					break;
				}
				else if (i >= arrowList.Count || arrowList[i].tar != taggedObjects[i])
				{
					Arrow arr = MakeArrowOb(arrowSprites[arrowType], taggedObjects[i]);
					arrowList.Insert(i, arr);
					arr.sprite.name = "Arrow " + i;
				}
				PositionArrow(arrowList[i]);
			}
		}

		/// <summary>
		/// Gives an arrow a position and rotation based on its position relative to the camera.
		/// </summary>
		/// <param name="arrow">The arrow to position</param>
		void PositionArrow(Arrow arrow)
		{
			xCenter = canvas.pixelRect.width / 2;
			yCenter = canvas.pixelRect.height / 2;
			float sw = canvas.pixelRect.width;
			float sh = canvas.pixelRect.height;
			screenSlope = sh / sw;

			halfSize = size / 2;

			float angle = hoverAngle - 180;
			float rad = angle * (Mathf.PI / 180.0f);
			Vector3 arrowPos = cam.transform.right * Mathf.Cos(rad) + cam.transform.up * Mathf.Sin(rad);
			Vector3 worldPos = arrow.tar.transform.position;
			Vector3 hPos = cam.WorldToViewportPoint(worldPos + (arrowPos * distanceAbove));
			Vector3 tarPos = cam.WorldToViewportPoint(worldPos);
			Vector3 pos;
			if (projectFromHoverPoint || (tarPos.z > 0 && tarPos.x >= 0 && tarPos.x <= 1 && tarPos.y >= 0 && tarPos.y <= 1))
			{
				pos = hPos;
			}
			else
			{
				pos = tarPos;
			}
			if (pos.z < 0)
			{
				pos.x *= -1;
				pos.y *= -1;
			}
			if ((pos.z > 0 || (pos.z < 0 && (pos.x > .5 + (blindSpot / 2) || pos.x < .5 - (blindSpot / 2))
							&& (pos.y < .5 - (blindSpot / 2) || pos.y > .5 + (blindSpot / 2)))))
			{
				float newX = pos.x * sw;
				float newY = pos.y * sh;
				//if the object is offscreen
				if (pos.z < 0 || (newY < 0 || newY > sh || newX < 0 || newX > sw))
				{
					if (projectToEdge)
					{
						if (projectFromHoverPoint)
						{
							newX += halfSize * Mathf.Cos(rad);
							newY += halfSize * Mathf.Sin(rad);
						}
						arrow.sprite.GetComponent<Image>().enabled = true;
						float a = CalculateAngle(sw / 2, sh / 2, newX, newY);
						Vector2 coord = ProjectToEdge(newX, newY);
						RectTransform rect = arrow.sprite.GetComponent<RectTransform>();
						rect.position = new Vector3(coord.x, coord.y, 0);
						rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
						rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
						rect.eulerAngles = new Vector3(0, 0, a);
					}
					else
					{
						arrow.sprite.GetComponent<Image>().enabled = false;
					}
				}
				else if (hoverOnScreen)
				{
					newX += halfSize * Mathf.Cos(rad);
					newY += halfSize * Mathf.Sin(rad);
					arrow.sprite.GetComponent<Image>().enabled = true;
					RectTransform rect = arrow.sprite.GetComponent<RectTransform>();
					rect.position = new Vector3(newX, newY, 0);
					rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
					rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
					rect.eulerAngles = new Vector3(0, 0, hoverAngle);
				}
				else
				{
					arrow.sprite.GetComponent<Image>().enabled = false;
				}



			}
			else
			{
				arrow.sprite.GetComponent<Image>().enabled = false;
			}
		}

		/// <summary>
		/// Use to instantiate a new arrow. Hooks it up with all needed
		/// components and makes it a child of the canvas (gameObject).
		/// </summary>
		/// <returns>Object referencing arrow's target and sprite objects</returns>
		/// <param name="arrow">Sprite to instantiate</param>
		/// <param name="target">What the arrow points to</param>
		Arrow MakeArrowOb(Sprite arrow, GameObject target)
		{
			GameObject ob = new GameObject();
			ob.transform.parent = gameObject.transform;
			ob.AddComponent<Image>();
			ob.GetComponent<Image>().sprite = arrow;
			ob.GetComponent<Image>().raycastTarget = false;
			Arrow theArrow = new Arrow(ob, target);
			return theArrow;
		}

		/// <summary>
		/// Calculates the angle between two 2D points
		/// </summary>
		/// <returns>The angle.</returns>
		/// <param name="x1">The first x value.</param>
		/// <param name="y1">The first y value.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="y2">The second y value.</param>
		float CalculateAngle(float x1, float y1, float x2, float y2)
		{
			float xDiff = x2 - x1;
			float yDiff = y2 - y1;
			float rad = Mathf.Atan(yDiff / xDiff);
			float deg = rad * 180 / Mathf.PI;
			if (xDiff < 0)
			{
				deg += 180;
			}
			return deg;
		}

		/// <summary>
		/// Projects to edge.
		/// </summary>
		/// <returns>A point going out from the center of the screen, but stopping at the edge.</returns>
		/// <param name="x2">Target x</param>
		/// <param name="y2">Target y</param>
		Vector2 ProjectToEdge(float x, float y)
		{
			float xDiff = x - (canvas.pixelRect.width / 2);
			float yDiff = y - (canvas.pixelRect.height / 2);
			float slope = yDiff / xDiff;
			Vector2 coord = new Vector2(0, 0);
			float ratio;
			if (slope > screenSlope || slope < -screenSlope)
			{
				//project on top/bottom
				ratio = (yCenter - halfSize) / yDiff;
				if (yDiff < 0)
				{
					coord.y = halfSize;
					ratio *= -1;
				}
				else coord.y = canvas.pixelRect.height - halfSize;
				coord.x = xCenter + xDiff * ratio;
			}
			else
			{
				//project on left/right
				ratio = (xCenter - halfSize) / xDiff;
				if (xDiff < 0)
				{
					coord.x = halfSize;
					ratio *= -1;
				}
				else coord.x = canvas.pixelRect.width - halfSize;
				coord.y = yCenter + yDiff * ratio;
			}
			return coord;
		}
	}

	/// <summary>
	/// A small class used only for referencing related scene objects
	/// </summary>
	public class Arrow
	{
		public GameObject sprite; //Arrow UI element object
		public GameObject tar; //the object the arrow points to

		public Arrow(GameObject sprite, GameObject tar)
		{
			this.sprite = sprite;
			this.tar = tar;
		}
	}
}