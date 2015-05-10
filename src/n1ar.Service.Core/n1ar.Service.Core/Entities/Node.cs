namespace n1ar.Service.Core.Entities {
    class Node {
        public Node() {

        }

        private Node _left;
        private Node _right;
        private string _value;

        public string Value {
            get {
                return _value;
            }
        }

        public Node LeftSubNode {
            get {
                return _left;
            }
            set {
                _left = value;
            }
        }

        public Node RightSubNode {
            get {
                return _right;
            }
            set {
                _right = value;
            }
        }

        public Node SetValue(string nodeValue) {
            _value = nodeValue;
            return this;
        }
    }
}
