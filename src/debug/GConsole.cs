using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotRx;
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

        class Command : Object
        {
            readonly List<(string name, Variant.Type type)> _arguments;
            readonly string _name, _description, _funcName;
            readonly Object _instance;

            public Command() : this(null, null, null, null)
            {
            }

            public Command(
                Node instance,
                string funcName,
                string name,
                string description,
                params (string, Variant.Type)[] args
            )
            {
                _instance = instance;
                _funcName = funcName;
                _name = name;
                _arguments = args.ToList();
                _description = description ?? string.Empty;

                instance
                    .OnTreeExiting()
                    .Subscribe(_ =>
                    {
                        Singleton.DebugPrint($"Removed Command {_name}");
                        Singleton.Call("remove_command", _name);
                    })
                    .DisposeWith(this);
            }

            public void Register()
            {
                var cmd = (Object) Singleton.Call("add_command", _name, _instance, _funcName);
                cmd = (Object) cmd.Call("set_description", _description);

                _arguments
                    .Aggregate(cmd, (chain, arg) =>
                        (Object) chain.Call("add_argument", arg.name, arg.type))
                    .Call("register");
            }
        }
    }
}