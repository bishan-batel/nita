using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Parry2.debug
{
    public static class GConsole
    {
        public static Node Singleton => Global.Singleton.GetNode("/root/Console");

        public static void WriteLine(string msg) => Singleton.Call("write_line", msg);
        public static void Write(string msg) => Singleton.Call("write", msg);
        public static void Clear() => Singleton.Call("clear");
        public static void ToggleConsole() => Singleton.Call("toggle_console");

        public static void AddCommand(
            this Node instance,
            string name,
            string description,
            string funcName,
            params (string name, Variant.Type type)[] args)
        {
            var cmd = new Command(instance, funcName, name, description, args);
            instance.AddChild(cmd);
            cmd.Register();
        }

        public class Command : Node
        {
            public readonly string Description;
            public readonly string FuncName;
            public readonly Godot.Object Instance;
            public readonly List<(string name, Variant.Type type)> Arguments;

            public Command(
                Godot.Object instance,
                string funcName,
                string name,
                string description,
                params (string, Variant.Type)[] args
            )
            {
                Instance = instance;
                FuncName = funcName;
                Name = name;
                Arguments = args.ToList();
                Description = description ?? string.Empty;
            }

            public void Register()
            {
                var cmd = (Godot.Object) Singleton.Call("add_command", Name, Instance, FuncName);
                cmd = (Godot.Object) cmd.Call("set_description", Description);

                Arguments
                    .Aggregate(cmd, (chain, arg) =>
                        (Godot.Object) chain.Call("add_argument", arg.name, arg.type))
                    .Call("register");
            }

            public void Remove() => Singleton.Call("remove_command", Name);
            public override void _ExitTree() => Remove();
        }
    }
}