using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T s_instance;

	public static T Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = FindObjectOfType<T>();
			}

			return s_instance;
		}
	}
	
	protected virtual bool OnEnable()
	{
		if (s_instance != null)
		{
			Debug.LogWarningFormat(this, "Singleton '{0}' is already instanced!", GetType().Name);

			enabled = false;

			return false;
		}

		s_instance = this as T;

		return true;
	}

	protected virtual bool OnDisable()
	{
		if (s_instance == this)
		{
			s_instance = null;

			return true;
		}

		return false;
	}
}
