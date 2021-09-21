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
            distanceRouters.Add(new DistanceVector(this.address, router.address, distance));
            ports.Add(router);
            router.connect(this, distance, router.address, true, version);
            foreach (DistanceVector distanceVector in distanceRouters) {
                if(distanceVector.to == router.address) {
                    continue;
                }
                //syncing is true now
                router.connect(this, distanceVector.distance, distanceVector.to, false, version, true);
            }
            foreach(Router rt in ports) {
                if(rt.address != router.address) {
                    rt.connect(this, distance, router.address, false, version);
                }
            }
        }
        public void connect(Router fromRouter, int distance, string to, bool addPort, int version, bool syncing = false, bool isNewRoute=false) {
            if(this.version == version && !syncing) {
                return;
            }
            this.version = version;
            //add to ports if not added
            if(addPort) {
                ports.Add(fromRouter);
            }
            foreach(DistanceVector dv in distanceRouters) {
                if(isNewRoute) {
                    break;
                }
                if((dv.to == fromRouter.address || dv.from == fromRouter.address
                && dv.to == this.address || dv.from == this.address) && !isNewRoute) {
                    distance+=dv.distance;
                    break;
                }
            }
            foreach(Router rt in ports) {
                if(rt.address != fromRouter.address) {
                    rt.connect(this, distance, to, false, version, false, true);
                }
            }
            distanceRouters.Add(new DistanceVector(fromRouter.address, to, distance));
        }

        public override string ToString() {
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
            routerA.ToString();
            routerB.ToString();
            routerC.ToString();
            routerD.ToString();
        }
    }
}