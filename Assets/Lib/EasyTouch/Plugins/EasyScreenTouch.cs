using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EasyScreenTouch : MonoBehaviour 
{
	#region Delegate
	public delegate void ScreenTouchUpHandler(Gesture gesture);
	public delegate void ScreenTouchPressHandler(Gesture gesture);
	public delegate void ScreenTouchDownHandler(Gesture gesture);
    public delegate void ScreenTouchSimpleTapHandler(Gesture gesture);

    public delegate void ScreenTouchJoystickUpHandler(Gesture gesture);
    public delegate void ScreenTouchJoystickPressHandler(Gesture gesture);
    public delegate void ScreenTouchJoystickDownHandler(Gesture gesture);
    #endregion

    #region Event
    public static event ScreenTouchUpHandler On_ScreenTouchDown;
	public static event ScreenTouchPressHandler On_ScreenTouchPress;
	public static event ScreenTouchDownHandler On_ScreenTouchUp;
    public static event ScreenTouchSimpleTapHandler On_ScreenSimpleTapDown;
    #endregion

    #region Enumerations
    public enum ScreenTouchAnchor {MiddleCenter};
	public enum Broadcast {SendMessage, SendMessageUpwards, BroadcastMessage }
	public enum ButtonState { Down, Press, Up, None};
	public enum InteractionType {Event, Include}
	private enum MessageName{On_ScreenTouchDown, On_ScreenTouchPress, On_ScreenTouchUp, On_ScreenSimpleTap};
	#endregion

	#region Members

	#region public members

	#region Button properties
	public bool enable = true;
	public bool isActivated = true;
	public bool showDebugArea=true;
	public bool selected=false;
	public bool isUseGuiLayout=true;
	public ButtonState buttonState = ButtonState.None;
	#endregion

	#region Button position & size
	[SerializeField]
	private ScreenTouchAnchor anchor = ScreenTouchAnchor.MiddleCenter;
	public ScreenTouchAnchor Anchor 
	{
		get
		{
			return this.anchor;
		}
		set 
		{
			anchor = value;
			ComputeButtonAnchor(anchor);
		}
	}	

	[SerializeField]
	private Vector2 offset = Vector2.zero;
	public Vector2 Offset
	{
		get 
		{
			return this.offset;
		}
		set 
		{
			offset = value;
			ComputeButtonAnchor(anchor);
		}
	}	

	[SerializeField]
	private Vector2 scale = Vector2.one;
	public Vector2 Scale 
	{
		get 
		{
			return this.scale;
		}
		set 
		{
			scale = value;
			ComputeButtonAnchor(anchor);
		}
	}	
		
	public bool isSwipeIn = false;
	public bool isSwipeOut = false;
	#endregion

	#region Interaction & Events
	public InteractionType interaction = InteractionType.Event;
	public bool useBroadcast = false;
	public GameObject receiverGameObject; 
	public Broadcast messageMode;
	public bool useSpecificalMethod = false;
	public string downMethodName;
	public string pressMethodName;
	public string upMethodName;
	#endregion

	public int guiDepth = 0;

	#region Inspector
	public bool showInspectorProperties = true;
	public bool showInspectorPosition = true;
	public bool showInspectorEvent = false;
	public bool showInspectorTexture = false;
	#endregion

	#endregion

	#region Private member
	private Rect screenTouchRect;
	private int screenTouchFingerIndex = -1;
	private Texture2D currentTexture;
	private int frame = 0;
	#endregion

	#endregion

	#region MonoBehaviour methods

	void OnEnable()
	{
		EasyTouch.On_TouchStart += On_TouchStart;
		EasyTouch.On_TouchDown += On_TouchDown;
		EasyTouch.On_TouchUp += On_TouchUp;
        EasyTouch.On_SimpleTap += On_SampleTap;

        // EasyTouch.On_Swipe += On_Swipe;
    }

	void OnDisable()
	{
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_TouchUp -= On_TouchUp;
        EasyTouch.On_SimpleTap -= On_SampleTap;

        if (Application.isPlaying)
		{
			if (EasyTouch.instance != null)
			{
				EasyTouch.instance.reservedVirtualAreas.Remove(screenTouchRect);
			}
		}		
	}

	public Rect GetBtnRect()
	{
		return screenTouchRect;
	}

	void OnDestroy()
	{
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_TouchUp -= On_TouchUp;	

		if (Application.isPlaying)
		{
			if (EasyTouch.instance != null)
			{
				EasyTouch.instance.reservedVirtualAreas.Remove(screenTouchRect);
			}
		}
	}

	void Start()
	{
		buttonState = ButtonState.None;
		VirtualScreen.ComputeVirtualScreen();
		ComputeButtonAnchor(anchor);		
	}

	void OnGUI()
	{
		if (enable)
		{
			GUI.depth = guiDepth;

			useGUILayout = isUseGuiLayout;

			VirtualScreen.ComputeVirtualScreen();
			VirtualScreen.SetGuiScaleMatrix();
		}
		else
		{
			if (Application.isPlaying)
				EasyTouch.instance.reservedVirtualAreas.Remove(screenTouchRect);
		}
	}

	void Update()
	{
		if (buttonState == ButtonState.Up)
		{
			buttonState = ButtonState.None;	
		}

		if (EasyTouch.GetTouchCount() == 0)
		{
			screenTouchFingerIndex = -1;
			buttonState = ButtonState.None;	
		}
	}

	void OnDrawGizmos()
	{
	}
	#endregion

	#region Private methods
	void ComputeButtonAnchor(ScreenTouchAnchor anchor)
	{
		Vector2 screenTouchSize = new Vector2(VirtualScreen.width, VirtualScreen.height);
		Vector2 anchorPosition = Vector2.zero;

		switch (anchor)
		{
		case ScreenTouchAnchor.MiddleCenter:
			anchorPosition = new Vector2( VirtualScreen.width / 2 - screenTouchSize.x / 2, VirtualScreen.height / 2 - screenTouchSize.y / 2);
			break;
		}
			
		screenTouchRect = new Rect(anchorPosition.x + offset.x, anchorPosition.y + offset.y, screenTouchSize.x, screenTouchSize.y);
	}

	void RaiseEvent(MessageName msg, Gesture gesture)
	{
		if (interaction == InteractionType.Event)
		{
			if (!useBroadcast)
			{
				switch (msg)
				{	
				case MessageName.On_ScreenTouchDown:
					if (On_ScreenTouchDown != null)
						On_ScreenTouchDown(gesture);
					break;
				case MessageName.On_ScreenTouchUp:
					if (On_ScreenTouchUp != null)
						On_ScreenTouchUp(gesture);	
					break;
				case MessageName.On_ScreenTouchPress:
					if (On_ScreenTouchPress != null)
						On_ScreenTouchPress(gesture);
					break;
                case MessageName.On_ScreenSimpleTap:
                    if (On_ScreenSimpleTapDown != null)
                        On_ScreenSimpleTapDown(gesture);
                    break;
                }
			}
			else
			{
				string method = msg.ToString();

				if (msg == MessageName.On_ScreenTouchDown && downMethodName!="" && useSpecificalMethod)
					method = downMethodName;		

				if (msg == MessageName.On_ScreenTouchPress && pressMethodName!="" && useSpecificalMethod)
					method = pressMethodName;				

				if (msg == MessageName.On_ScreenTouchUp && upMethodName!="" && useSpecificalMethod)
					method = upMethodName;		

				if (receiverGameObject != null)
				{		
					switch(messageMode)
					{
					case Broadcast.BroadcastMessage:
						receiverGameObject.BroadcastMessage(method, name, SendMessageOptions.DontRequireReceiver);
						break;
					case Broadcast.SendMessage:
						receiverGameObject.SendMessage(method, name, SendMessageOptions.DontRequireReceiver);
						break;
					case Broadcast.SendMessageUpwards:
						receiverGameObject.SendMessageUpwards(method, name, SendMessageOptions.DontRequireReceiver);
						break;
					}	
				}
				else
				{
					Debug.LogError("Button : " + gameObject.name + " : you must setup receiver gameobject");		
				}
			}
		}
	}
	#endregion

	#region EasyTouch Event
	void On_TouchStart (Gesture gesture)
	{
		if (gesture.IsInRect(VirtualScreen.GetRealRect(EasyJoystick.RectOperation), true))
		{
			//Debug.Log("Joystick Area Start");
			return;
		}

		if (gesture.IsInRect( VirtualScreen.GetRealRect(screenTouchRect), true)
			&& enable 
			&& isActivated)
		{
			screenTouchFingerIndex = gesture.fingerIndex;
			buttonState = ButtonState.Down;
			frame = 0;

			//Debug.Log("MessageName.On_ScreenTouchDown");
			RaiseEvent(MessageName.On_ScreenTouchDown, gesture);
		}
	}

	void On_TouchDown (Gesture gesture)
	{
		if (gesture.IsInRect(VirtualScreen.GetRealRect(EasyJoystick.RectOperation), true))
		{
			//Debug.Log("Joystick Area Down");
			return;
		}

		if (gesture.fingerIndex == screenTouchFingerIndex 
			|| (isSwipeIn && buttonState == ButtonState.None))
		{
			if (gesture.IsInRect( VirtualScreen.GetRealRect(screenTouchRect), true) 
				&& enable 
				&& isActivated)
			{	
				frame++;

				if ((buttonState == ButtonState.Down 
					|| buttonState == ButtonState.Press) 
					&& frame >= 2)
				{ 

					//Debug.Log("MessageName.On_ScreenTouchPress");
					RaiseEvent(MessageName.On_ScreenTouchPress, gesture);	
					buttonState = ButtonState.Press;
				}

				if (buttonState == ButtonState.None)
				{
					screenTouchFingerIndex = gesture.fingerIndex;
					buttonState = ButtonState.Down;
					frame = 0;

					//Debug.Log("MessageName.On_ScreenTouchDown");
					RaiseEvent(MessageName.On_ScreenTouchDown, gesture);
				}
			}
			else 
			{
				if (((isSwipeIn || !isSwipeIn ) && !isSwipeOut) 
					&& buttonState == ButtonState.Press)
				{
					screenTouchFingerIndex = -1;
					buttonState = ButtonState.None;					
				}
				else if (isSwipeOut && buttonState == ButtonState.Press) 
				{
					//Debug.Log("MessageName.On_ScreenTouchPress");

					RaiseEvent(MessageName.On_ScreenTouchPress, gesture);
					buttonState = ButtonState.Press;
				}
			}
		}

	}

    void On_TouchUp(Gesture gesture)
    {
        if (gesture.IsInRect(VirtualScreen.GetRealRect(EasyJoystick.RectOperation), true))
        {
            //Debug.Log("Joystick Area Up");
            return;
        }

        if (gesture.fingerIndex == screenTouchFingerIndex)
        {
            if ((gesture.IsInRect(VirtualScreen.GetRealRect(screenTouchRect), true)
                || (isSwipeOut && buttonState == ButtonState.Press))
                && enable && isActivated)
            {

                //Debug.Log("MessageName.On_ScreenTouchUp");

                RaiseEvent(MessageName.On_ScreenTouchUp, gesture);
            }

            buttonState = ButtonState.Up;
            screenTouchFingerIndex = -1;
        }
    }

    void On_SampleTap (Gesture gesture)
	{
        if ((gesture.IsInRect(VirtualScreen.GetRealRect(EasyJoystick.RectOperation), true))
                && enable && isActivated)
        {
            RaiseEvent(MessageName.On_ScreenSimpleTap, gesture);
        }
    }
    #endregion
}
