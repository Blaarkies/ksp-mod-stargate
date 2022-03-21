namespace Stargate.Domain
{
    public class Sounds
    {
        public static SoundPack ChevronLock = new SoundPack("chevron_lock", 2);
        public static SoundPack Chevron = new SoundPack("chevron", 6);
        public static SoundPack DhdFail = new SoundPack("dhd_fail", 1);
        public static SoundPack Dhd = new SoundPack("dhd", 7);
        public static SoundPack Fail = new SoundPack("fail", 1);
        public static SoundPack GateClose = new SoundPack("gate_close", 6);
        public static SoundPack GateDialFail = new SoundPack("gate_dial_fail", 1);
        public static SoundPack GateOpen = new SoundPack("gate_open", 4);
        public static SoundPack GateRollLong = new SoundPack("gate_roll_long", 1);
        public static SoundPack GateRollLoop = new SoundPack("gate_roll_loop", 1, true);
        public static SoundPack GateRollShort = new SoundPack("gate_roll_short", 2);
        public static SoundPack RingsTransport = new SoundPack("rings_transport", 4);
        public static SoundPack EventhorizonLoop = new SoundPack("wormhole_eventhorizon_loop", 1, true);
        public static SoundPack WormholeStep = new SoundPack("wormhole_step", 5);
        public static SoundPack WormholeTravel = new SoundPack("wormhole_travel", 1);
    }
}
