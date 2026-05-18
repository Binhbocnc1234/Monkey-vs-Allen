// using UnityEngine;
// using System;

// public interface IEventArgs{

// }
// public class MyEvent<T, U> where U : IEventArgs{

// }
// public static class EventHanlder{
//     public static void AddOnce(ref EventHandler evt, EventHandler handler){
//         EventHandler wrapper = null;
//         wrapper = (sender, e) => {
//             handler(sender, e);
//             evt -= wrapper;
//         };
//         evt += wrapper;
//     }
// } 


// public static class OneTimeEventListener {
//     // For Action (no args)
//     public static void AddOnce(ref Action evt, Action handler) {
//         Action wrapper = null;
//         wrapper = () => {
//             handler();
//             evt -= wrapper;
//         };
//         evt += wrapper;
//     }

//     // For Action<T>
//     public static void AddOnce<T>(ref Action<T> evt, Action<T> handler) {
//         Action<T> wrapper = null;
//         wrapper = (arg) => {
//             handler(arg);
//             evt -= wrapper;
//         };
//         evt += wrapper;
//     }

//     // For EventHandler (object sender, EventArgs e)
//     public static void AddOnce(EventHandler evt, EventHandler handler) {
//         EventHandler wrapper = null;
//         wrapper = (sender, e) => {
//             handler(sender, e);
//             evt -= wrapper;
//         };
//         evt += wrapper;
//     }

//     // For EventHandler<TEventArgs>
//     public static void AddOnce<TEventArgs>(ref EventHandler<TEventArgs> evt, EventHandler<TEventArgs> handler)
//         where TEventArgs : EventArgs {
//         EventHandler<TEventArgs> wrapper = null;
//         wrapper = (sender, e) => {
//             handler(sender, e);
//             evt -= wrapper;
//         };
//         evt += wrapper;
//     }
// }
