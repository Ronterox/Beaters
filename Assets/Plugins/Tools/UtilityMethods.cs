using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Plugins.Tools
{
    public class UtilityMonoBehaviour : MonoBehaviour { }

    /// <summary>
    /// A class with useful methods for every type of special case
    /// </summary>
    public static class UtilityMethods
    {
        public static UtilityMonoBehaviour monoBehaviour;

        /// <summary>
        /// Obtains a monobehaviour if existent else creates one
        /// </summary>
        /// <returns>the monobehaviour</returns>
        private static UtilityMonoBehaviour GetMonoBehaviour() => monoBehaviour ? monoBehaviour : monoBehaviour = new GameObject { name = "UtilityMethods_GameObject" }.AddComponent<UtilityMonoBehaviour>();


        /// <summary>
        /// Obtains a monobehaviour if existent else creates one, but also makes it persistent
        /// </summary>
        /// <returns>the monobehaviour</returns>
        private static UtilityMonoBehaviour GetPersistentMonoBehaviour()
        {
            UtilityMonoBehaviour behaviour = GetMonoBehaviour();
            UnityEngine.Object.DontDestroyOnLoad(behaviour.gameObject);
            return behaviour;
        }

        /// <summary>
        /// Activates or deactivates the children the myGameObject
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="active">Whether to activate or deactivate its children</param>
        public static void SetActiveChildren(this GameObject parent, bool active = true)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(active)) child.gameObject.SetActive(active);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Instantiates an actual prefab instead of a myGameObject
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        public static GameObject InstantiatePrefab(GameObject prefab, Vector2 position, Quaternion rotation,
            Transform parent = null)
        {
            if (!(PrefabUtility.InstantiatePrefab(prefab) is GameObject result)) return null;
            result.transform.position = position;
            result.transform.parent = parent;
            result.transform.rotation = rotation;
            return result;
        }
#endif
        /// <summary>
        /// Returns the degrees as radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float ToRadians(this float degrees) => Mathf.Rad2Deg * degrees;

        /// <summary>
        /// Returns the string as a smart color string
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToColorString(this string sentence, string color) => $"<color={color}>{sentence}</color>";

        /// <summary>
        /// Returns a random direction Vector3
        /// </summary>
        /// <param name="horizontalDirections"></param>
        /// <param name="verticalDirections"></param>
        /// <returns></returns>
        public static Vector3 GetRandomDirection(bool horizontalDirections, bool verticalDirections) => Random.Range(horizontalDirections ? 0 : 4, verticalDirections ? 6 : 4) switch { 0 => Vector3.forward, 1 => Vector3.back, 2 => Vector3.left, 3 => Vector3.right, 4 => Vector3.up, 5 => Vector3.down, _ => Vector3.zero };

        /// <summary>
        /// Returns the float, as a percentage of the max value, from 0 to 1;
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float GetPercentageValue(this float value, float maxValue) => value / maxValue;

        /// <summary>
        /// Returns a random vector2 with points between the values passed
        /// </summary>
        /// <param name="minValueX"></param>
        /// <param name="maxValueX"></param>
        /// <param name="minValueY"></param>
        /// <param name="maxValueY"></param>
        /// <returns></returns>
        public static Vector2 GetRandomVector2(float minValueX, float maxValueX, float minValueY, float maxValueY) => new Vector2(Random.Range(minValueX, maxValueX), Random.Range(minValueY, maxValueY));

        /// <summary>
        /// Changes the value by a limited amount
        /// </summary>
        /// <param name="value"></param>
        /// <param name="increment">Positive or negative value to be incremented or decremented with</param>
        /// <param name="byLimit">the max amount to be limited to</param>
        public static void ChangeValueLimited(this ref float value, float increment, float byLimit) => value = (value + increment) % byLimit;

        /// <summary>
        /// Changes the value by a limited amount
        /// </summary>
        /// <param name="value"></param>
        /// <param name="increment">Positive or negative value to be incremented or decremented with</param>
        /// <param name="byLimit">the max amount to be limited to</param>
        public static void ChangeValueLimited(this ref int value, int increment, int byLimit) => value = (value + increment) % byLimit;

        /// <summary>
        /// Coroutine standard cycle function to be reused
        /// </summary>
        /// <param name="whileCondition"></param>
        /// <param name="loopWaitForSeconds"></param>
        /// <param name="loopAction"></param>
        /// <param name="onceStartCoroutine"></param>
        /// <param name="onceFinishCoroutine"></param>
        /// <returns></returns>
        public static IEnumerator FunctionCycleCoroutine(this Func<bool> whileCondition, Action loopAction, WaitForSeconds loopWaitForSeconds = null, Action onceStartCoroutine = null, Action onceFinishCoroutine = null)
        {
            onceStartCoroutine?.Invoke();
            while (whileCondition())
            {
                loopAction?.Invoke();
                yield return loopWaitForSeconds;
            }
            onceFinishCoroutine?.Invoke();
        }

        /// <summary>
        /// Delays an action, by a quantity of seconds
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        public static void DelayAction(this Action action, float delay) => GetMonoBehaviour().StartCoroutine(DelayActionCoroutine(action, delay));

        /// <summary>
        /// A coroutine that delays an action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static IEnumerator DelayActionCoroutine(this Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        /// <summary>
        /// Checks if a float approximates other float by a tolerance
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objective"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool Approximates(this float value, float objective, float tolerance = 0.01f) => Math.Abs(value - objective) <= tolerance;


        /// <summary>
        /// Checks if a vector3 approximates other vector3 by a tolerance
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objective"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool Approximates(this Vector3 value, Vector3 objective, float tolerance = 0.01f) => Vector3.SqrMagnitude(value - objective) <= tolerance;

        /// <summary>
        /// Lerps a vector3
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objective"></param>
        /// <param name="speed">Between 0 and 1</param>
        public static void Lerp(this ref Vector3 value, Vector3 objective, float speed) => value = Vector3.Lerp(value, objective, speed);

        /// <summary>
        /// Lerps a float
        /// </summary>
        /// <param name="value"></param>
        /// <param name="objective"></param>
        /// <param name="speed">Between 0 and 1</param>
        public static void Lerp(this ref float value, float objective, float speed) => value = Mathf.Lerp(value, objective, speed);

        /// <summary>
        /// Moves the myGameObject to the exist if exist, if it doesn't it creates the scene
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sceneName"></param>
        public static void MoveToScene(this GameObject obj, string sceneName)
        {
            if (obj.transform.parent) return;
            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.MoveGameObjectToScene(obj, scene.IsValid() ? scene : SceneManager.CreateScene(sceneName));
        }

        /// <summary>
        /// Returns the decibel value as a volume value from 0 to 1
        /// </summary>
        /// <param name="dB"></param>
        /// <returns></returns>
        public static float ToVolume(this float dB) => Mathf.Pow(10, dB * 0.05f);

        /// <summary>
        /// Returns the volume from 0 to 1 as its decibel value
        /// </summary>
        /// <param name="volume">from 0 to 1</param>
        /// <returns>volume on decibels</returns>
        public static float ToDecibels(this float volume) => volume > 0 ? Mathf.Log10(volume) * 20 : -80f;

        /// <summary>
        /// Returns the component of the myGameObject if it founds it, else adds the component and returns it
        /// </summary>
        /// <param name="myGameObject">myGameObject</param>
        /// <typeparam name="T">component type</typeparam>
        /// <returns></returns>
        public static T GetComponentSafely<T>(this GameObject myGameObject) where T : Component => myGameObject.TryGetComponent(out T component) ? component : myGameObject.AddComponent<T>();


        /// <summary>
        /// Rotates a transform towards the other transform by only the selected axis
        /// </summary>
        /// <param name="transform">my transform</param>
        /// <param name="otherTransform">other transform</param>
        /// <param name="axis">axis to rotate Example: Vector3.up for Y axis</param>
        public static void RotateTowards(this Transform transform, Transform otherTransform, [DefaultValue("Vector3.up")] Vector3 axis) => RotateTowards(transform, otherTransform.position, axis);

        /// <summary>
        /// Rotates a transform towards the other transform by only the selected axis
        /// </summary>
        /// <param name="transform">my transform</param>
        /// <param name="direction">target direction</param>
        /// <param name="axis">axis to rotate Example: Vector3.up for Y axis</param>
        public static void RotateTowards(this Transform transform, Vector3 direction, [DefaultValue("Vector3.up")] Vector3 axis)
        {
            Vector3 position = transform.position;

            if (axis.x != 0) direction.x = position.x;
            else if (axis.y != 0) direction.y = position.y;
            else if (axis.z != 0) direction.z = position.z;

            transform.LookAt(direction);
        }

        /// <summary>
        /// Rotates a transform towards the other transform only by the y axis
        /// </summary>
        /// <param name="transform">my transform</param>
        /// <param name="otherTransform">other transform</param>
        public static void RotateTowards(this Transform transform, Transform otherTransform) => RotateTowards(transform, otherTransform, Vector3.up);
        /// <summary>
        /// Rotates a transform towards the other transform only by the y axis
        /// </summary>
        /// <param name="transform">my transform</param>
        /// <param name="direction">target direction</param>
        public static void RotateTowards(this Transform transform, Vector3 direction) => RotateTowards(transform, direction, Vector3.up);

        /// <summary>
        /// Best random shuffle method
        /// </summary>
        /// <param name="rng">seed of randomness</param>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static T[] Shuffle<T>(this T[] array, System.Random rng)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
            return array;
        }

        /// <summary>
        /// Best random shuffle method
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static T[] Shuffle<T>(this T[] array) => Shuffle(array, new System.Random());

        /// <summary>
        /// Foreach extension method for arrays
        /// </summary>
        /// <param name="array"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this IEnumerable<T> array, Action<T> action)
        {
            foreach (T x1 in array) action.Invoke(x1);
        }

        /// <summary>
        /// Gets a random value from an array
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandom<T>(this T[] array) => array[Random.Range(0, array.Length)];

        /// <summary>
        /// Gets a random value from a List
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRandom<T>(this List<T> array) => array[Random.Range(0, array.Count)];

        /// <summary>
        /// Returns the negative value of a positive and the other way around
        /// </summary>
        /// <returns></returns>
        public static float ContraryValue(this float value) => value > 0 ? -value : value * -1;
    }
}
