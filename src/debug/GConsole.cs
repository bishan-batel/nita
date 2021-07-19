using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Parry2.utils;
using Object = Godot.Object;

namespace Parry2.debug
{
    public static class GConsole
    {
        public static Node Singleton => Global.Singleton?.GetNode("/root/Console");

        public static void WriteLine(string msg)
        {
            Singleton.Call("write_line", msg);
        }

        public static void Write(string msg)
        {
            Singleton.Call("write", msg);
        }

        public static void Clear()
        {
            Singleton.Call("clear");
        }

        public static void ToggleConsole()
        {
            Singleton.Call("toggle_console");
        }

        public static void AddCommand(
            this Node instance,
            string name,
            string description,
            string funcName,
            params (string name, Variant.Type type)[] args)
        {
            new Command(instance, funcName, name, description, args)
                .Register();
        }

        public class Command : Object
        {
            public readonly List<(string name, Variant.Type type)> Arguments;
            public readonly string Name, Description, FuncName;
            public readonly Object Instance;

            public Command() : this(null, null, null, null)
            {
            }

            public Command(
                Object instance,
                string funcName,
                string name,
                string description,
                params (string, Variant.Type)[] args
            )
            {
                Instance = instance;
                instance.Connect("tree_exited", this, nameof(Remove));
                FuncName = funcName;
                Name = name;
                Arguments = args.ToList();
                Description = description ?? string.Empty;
            }

            public void Register()
            {
                var cmd = (Object) Singleton.Call("add_command", Name, Instance, FuncName);
                cmd = (Object) cmd.Call("set_description", Description);

                Arguments
                    .Aggregate(cmd, (chain, arg) =>
                        (Object) chain.Call("add_argument", arg.name, arg.type))
                    .Call("register");
            }

            public void Remove()
            {
                if (Singleton is null || !Singleton.IsInsideTree()) return;
                Singleton.DebugPrint($"Removed Command {Name}");
                Singleton.Call("remove_command", Name);
            }
        }
    }
}