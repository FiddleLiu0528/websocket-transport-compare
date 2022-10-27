/*eslint-disable block-scoped-var, id-length, no-control-regex, no-magic-numbers, no-prototype-builtins, no-redeclare, no-shadow, no-var, sort-vars*/
import * as $protobuf from "protobufjs/light";

const $root = ($protobuf.roots["default"] || ($protobuf.roots["default"] = new $protobuf.Root()))
.addJSON({
  base: {
    nested: {
      Base: {
        fields: {
          action: {
            type: "string",
            id: 1
          }
        }
      }
    }
  },
  first: {
    nested: {
      Request: {
        fields: {
          action: {
            type: "string",
            id: 1
          },
          mode: {
            type: "uint32",
            id: 2
          },
          pt: {
            type: "int32",
            id: 3
          },
          sort: {
            type: "uint32",
            id: 4
          },
          keepL: {
            type: "uint32",
            id: 5
          },
          dc: {
            type: "uint32",
            id: 6
          }
        }
      },
      Response: {
        fields: {
          action: {
            type: "string",
            id: 1
          },
          menu: {
            rule: "repeated",
            type: "uint32",
            id: 2
          },
          index: {
            rule: "repeated",
            type: "uint32",
            id: 3
          },
          data: {
            rule: "repeated",
            type: "Data",
            id: 4
          },
          alMenu: {
            rule: "repeated",
            type: "uint32",
            id: 5
          },
          alData: {
            type: "Aldata",
            id: 6
          },
          dc: {
            type: "uint32",
            id: 7
          }
        },
        nested: {
          Scht: {
            fields: {
              tnA: {
                type: "string",
                id: 1
              },
              tnB: {
                type: "string",
                id: 2
              },
              tcA: {
                type: "string",
                id: 3
              },
              tcB: {
                type: "string",
                id: 4
              }
            }
          },
          Data: {
            fields: {
              schId: {
                type: "uint32",
                id: 1
              },
              mode: {
                type: "uint32",
                id: 2
              },
              pt: {
                type: "uint32",
                id: 3
              },
              people: {
                type: "uint32",
                id: 4
              },
              schT: {
                type: "Scht",
                id: 5
              },
              liveId: {
                type: "uint32",
                id: 6
              },
              liveName: {
                type: "string",
                id: 7
              },
              liveLang: {
                type: "string",
                id: 8
              },
              alName: {
                type: "string",
                id: 9
              }
            }
          },
          Aldata: {
            fields: {
              pt: {
                rule: "repeated",
                type: "string",
                id: 1
              }
            }
          }
        }
      }
    }
  },
  register: {
    nested: {
      Request: {
        fields: {
          action: {
            type: "string",
            id: 1
          },
          id: {
            type: "string",
            id: 2
          }
        }
      },
      Response: {
        fields: {
          action: {
            type: "string",
            id: 1
          },
          id: {
            type: "string",
            id: 2
          }
        }
      }
    }
  }
});

export { $root as default };
