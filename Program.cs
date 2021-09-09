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
        public string address;
        public List<DistanceVector> distanceRouters = new List<DistanceVector>();
        public List<Router> ports = new List<Router>();

        public Router(string address) {
            this.address = address;
        }

        public void addRouter(Router router, int distance) {
            distanceRouters.Add(new DistanceVector(this.address, router.address, distance));
            ports.Add(router);
            router.connect(this, distance, router.address, true);
            foreach (DistanceVector distanceVector in distanceRouters) {
                if(distanceVector.to == router.address) {
                    continue;
                }
                router.connect(this, distanceVector.distance, distanceVector.from, false);

            }
            foreach(Router rt in ports) {
                if(rt.address != router.address) {
                    rt.connect(this, distance, router.address, false);
                }
            }
        }
        public void connect(Router fromRouter, int distance, string to, bool addPort) {
            //add to ports if not added
            if(addPort) {
                ports.Add(fromRouter);
            }
            foreach(DistanceVector dv in distanceRouters) {
                if(dv.to == fromRouter.address || dv.from == fromRouter.address
                && dv.to == this.address || dv.from == this.address) {
                    distance+=dv.distance;
                    break;
                }
            }
            foreach(Router rt in ports) {
                if(rt.address != fromRouter.address && rt.address != this.address) {
                    rt.connect(this, distance, to, false);
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
            Router router1 = new Router("A");
            Router router2 = new Router("B");
            Router router3 = new Router("C");
            Router router4 = new Router("D");
            router1.addRouter(router2, 2);
            router2.addRouter(router3, 1);
            router3.addRouter(router4, 3);
            router1.ToString();
            router2.ToString();
            router3.ToString();
            router4.ToString();
        }
    }
}