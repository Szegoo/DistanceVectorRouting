using System;
using System.Collections.Generic;

namespace Routing
{
    class DistanceVector {
        public string from;
        public string to;
        public int distance;
        public DistanceVector(string from, string to,int distance) {
            this.from = from;
            this.to = to;
            this.distance = distance;
        }
    }
    class Router {
        int version = 0;
        public string address;
        public List<DistanceVector> distanceRouters = new List<DistanceVector>();
        public List<Router> ports = new List<Router>();

        public Router(string address) {
            this.address = address;
        }

        public void addRouter(Router router, int distance) {
            version++;
            ports.Add(router);
            router.connect(this, distance, router.address, true, version);
            foreach(Router rt in ports) {
                if(rt.address != router.address) {
                    rt.connect(this, distance, router.address, false, version);
                }
            }
            sync(router, distance);
            distanceRouters.Add(new DistanceVector(this.address, router.address, distance));
        }
        public void connect(Router fromRouter, int distance, string to, bool addPort, int version, bool syncing = false) {
            if(this.version == version && !syncing) {
                return;
            }
            this.version = version;
            //does the router already have a path
            bool hasPath = hasRoute(fromRouter.address);
            //add to ports if not added
            if(addPort) {
                ports.Add(fromRouter);
            }
            if(!addPort) {
                foreach(DistanceVector dv in distanceRouters) {
                    if(dv.to == fromRouter.address) {
                        if(hasPath && (dv.from == fromRouter.address || dv.from == this.address)) {
                            distance+=dv.distance;
                            break;
                        }
                    }
                }
            }

            foreach(Router rt in ports) {
                if(rt.address != fromRouter.address) {
                    if(to == this.address) {
                        rt.connect(this, distance, fromRouter.address, false, version);
                    }else {
                        rt.connect(this, distance, to, false, version);
                    }
                }
            }
            if(to == this.address) {
                distanceRouters.Add(new DistanceVector(this.address, fromRouter.address, 
                distance));
            }else if(this.address != fromRouter.address) {
                distanceRouters.Add(new DistanceVector(fromRouter.address,to, 
                distance));
            }else {
                distanceRouters.Add(new DistanceVector(this.address,to, 
                distance));
            }
            if(addPort) { 
                sync(fromRouter, distance);
            }
        }
        public void sync(Router router, int distance) {
            foreach (DistanceVector distanceVector in distanceRouters) {
                if(distanceVector.to != router.address) {
                    //syncing is true now
                    router.connect(this, distanceVector.distance+distance, distanceVector.to, false, version, true);
                }
            }
        }
        public bool hasRoute(string to) {
            bool res = false;
            foreach(DistanceVector dv in distanceRouters) {
                if(dv.to == to){
                    res = true;
                    break;
                }
            }
            return res;
        }
        public override string ToString() {
            System.Console.WriteLine($"Version: {version}");
            string res = $"Router {address}: \n";
            foreach (DistanceVector dv in distanceRouters) {
                res += $"[{dv.from}][{dv.to}]"+dv.distance+"\n";
            }
            res += $"\nPorts {address}: \n";
            foreach(Router port in this.ports) {
                res += $"[{port.address}]\n";
            }
            System.Console.WriteLine(res);
            return res;
        }
        public void shortest(string to) {
            int min = -1;
            string from="";
            foreach (DistanceVector dv in distanceRouters) {
                if(dv.to == to) {
                    if(min == -1){ 
                        min = dv.distance;
                        from = dv.from;
                    }else if(dv.distance < min) {
                        min = dv.distance;
                        from = dv.from;
                    }
                }
            }
            System.Console.WriteLine($"Za {this.address} je minimum {from} do {to}: {min}\n");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Router routerA = new Router("A");
            Router routerB = new Router("B");
            Router routerC = new Router("C");
            Router routerD = new Router("D");
            routerA.addRouter(routerB, 2);
            routerB.addRouter(routerC, 1);
            routerC.addRouter(routerD, 3);
            routerD.addRouter(routerB, 1);
            routerA.addRouter(routerC, 2);

            routerA.shortest("B");
            routerA.shortest("C");
            routerA.shortest("D");
            System.Console.WriteLine("-------");
            routerB.shortest("A");
            routerB.shortest("C");
            routerB.shortest("D");
            System.Console.WriteLine("-------");
            routerC.shortest("A");
            routerC.shortest("B");
            routerC.shortest("D");
            System.Console.WriteLine("-------");
            routerD.shortest("A");
            routerD.shortest("B");
            routerD.shortest("C");
        }
    }
}