(self.webpackChunkant_design_pro=self.webpackChunkant_design_pro||[]).push([[604],{16894:function(C,h,a){"use strict";var r=a(65808),_=a(68129);h.ZP=_.Z},11974:function(C,h,a){"use strict";a.r(h);var r=a(11849),_=a(34792),c=a(48086),d=a(71194),I=a(53172),f=a(39428),D=a(3182),v=a(49684),b=a(41180),M=a(90774),x=a(46298),O=a(16894),L=a(67294),T=a(25377),A=a(73727),Z=a(74754),P=a(85893),y=function(){var R=(0,L.useRef)(),u=(0,T.YB)(),W=[{dataIndex:"workflowDefinitionId",title:u.formatMessage({id:"page.instance.field.definition"}),hideInTable:!0,valueType:"select",request:function(){var k=(0,D.Z)((0,f.Z)().mark(function o(E){var n,t,s;return(0,f.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.next=2,(0,b.Jx)({filter:(n=E.keyWords)!==null&&n!==void 0?n:"",maxResultCount:100});case 2:return s=e.sent,e.abrupt("return",((t=s.items)!==null&&t!==void 0?t:[]).map(function(i){return{label:"".concat(i.displayName,"(").concat(i.name,")"),value:i.id}}));case 4:case"end":return e.stop()}},o)}));function g(o){return k.apply(this,arguments)}return g}(),fieldProps:{showSearch:!0}},{dataIndex:"name",title:u.formatMessage({id:"page.instance.field.name"}),copyable:!0,render:function(g,o){return(0,P.jsx)(A.rU,{to:{pathname:"/instances/".concat(o.id)},children:g})}},{dataIndex:"version",title:u.formatMessage({id:"page.instance.field.version"}),valueType:"digit",width:80},{dataIndex:"workflowStatus",title:u.formatMessage({id:"page.instance.field.workflowStatus"}),valueEnum:Z.J},{dataIndex:"creationTime",title:u.formatMessage({id:"common.dict.creationTime"}),valueType:"dateTime",search:!1},{dataIndex:"finishedTime",title:u.formatMessage({id:"page.instance.field.finishedTime"}),valueType:"dateTime",search:!1},{dataIndex:"lastExecutedTime",title:u.formatMessage({id:"page.instance.field.lastExecutedTime"}),valueType:"dateTime",search:!1},{dataIndex:"faultedTime",title:u.formatMessage({id:"page.instance.field.faultedTime"}),valueType:"dateTime",search:!1},{dataIndex:"correlationId",title:u.formatMessage({id:"page.instance.field.correlationId"}),width:150,ellipsis:!0,copyable:!0},{title:u.formatMessage({id:"common.dict.table-action"}),valueType:"option",width:170,align:"center",render:function(g,o,E,n){var t=[];return(o.workflowStatus==v.x8.Idle||o.workflowStatus==v.x8.Running||o.workflowStatus==v.x8.Suspended)&&t.push((0,P.jsx)("a",{onClick:function(){I.Z.confirm({title:u.formatMessage({id:"page.instance.cancel.confirm.title"}),content:u.formatMessage({id:"page.instance.cancel.confirm.content"}),onOk:function(){var l=(0,D.Z)((0,f.Z)().mark(function i(){var w,m;return(0,f.Z)().wrap(function(p){for(;;)switch(p.prev=p.next){case 0:return p.next=2,(0,M.aT)(o.id);case 2:m=p.sent,m!=null&&(w=m.response)!==null&&w!==void 0&&w.ok&&(c.ZP.success(u.formatMessage({id:"page.instance.cancel.confirm.success"})),n==null||n.reload());case 4:case"end":return p.stop()}},i)}));function e(){return l.apply(this,arguments)}return e}()})},children:u.formatMessage({id:"page.instance.cancel"})},"cancel")),o.workflowStatus==v.x8.Faulted&&t.push((0,P.jsx)("a",{onClick:function(){I.Z.confirm({title:u.formatMessage({id:"page.instance.retry.confirm.title"}),content:u.formatMessage({id:"page.instance.retry.confirm.content"}),onOk:function(){var l=(0,D.Z)((0,f.Z)().mark(function i(){var w,m;return(0,f.Z)().wrap(function(p){for(;;)switch(p.prev=p.next){case 0:return p.next=2,(0,M.C4)(o.id,{});case 2:m=p.sent,m!=null&&(w=m.response)!==null&&w!==void 0&&w.ok&&(c.ZP.success(u.formatMessage({id:"page.instance.retry.confirm.success"})),n==null||n.reload());case 4:case"end":return p.stop()}},i)}));function e(){return l.apply(this,arguments)}return e}()})},children:u.formatMessage({id:"page.instance.retry"})},"retry")),t.push((0,P.jsx)("a",{onClick:function(){I.Z.confirm({title:u.formatMessage({id:"common.dict.delete.confirm"}),content:u.formatMessage({id:"page.instance.delete.confirm.content"}),onOk:function(){var l=(0,D.Z)((0,f.Z)().mark(function i(){var w,m;return(0,f.Z)().wrap(function(p){for(;;)switch(p.prev=p.next){case 0:return p.next=2,(0,M.BK)(o.id);case 2:m=p.sent,m!=null&&(w=m.response)!==null&&w!==void 0&&w.ok&&(c.ZP.success(u.formatMessage({id:"common.dict.delete.success"})),n==null||n.reload());case 4:case"end":return p.stop()}},i)}));function e(){return l.apply(this,arguments)}return e}()})},children:u.formatMessage({id:"common.dict.delete"})},"delete")),t}}];return(0,P.jsx)(x.ZP,{children:(0,P.jsx)(O.ZP,{columns:W,actionRef:R,search:{labelWidth:120},rowKey:"id",request:function(){var k=(0,D.Z)((0,f.Z)().mark(function g(o){var E,n,t,s;return(0,f.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return E=o.current,n=o.pageSize,delete o.current,delete o.pageSize,t=(E-1)*n,e.next=6,(0,M.qd)((0,r.Z)((0,r.Z)({},o),{},{skipCount:t,maxResultCount:n}));case 6:if(s=e.sent,!s){e.next=11;break}return e.abrupt("return",{success:!0,data:s.items,total:s.totalCount});case 11:return e.abrupt("return",{success:!1});case 12:case"end":return e.stop()}},g)}));return function(g){return k.apply(this,arguments)}}()})})};h.default=y},74754:function(C,h,a){"use strict";a.d(h,{J:function(){return I}});var r=a(32059),_=a(49684),c=a(25377),d,I=(d={},(0,r.Z)(d,_.x8.Idle,{text:(0,c.wv)({id:"page.instance.status.idle"}),status:"default"}),(0,r.Z)(d,_.x8.Running,{text:(0,c.wv)({id:"page.instance.status.running"}),status:"processing"}),(0,r.Z)(d,_.x8.Finished,{text:(0,c.wv)({id:"page.instance.status.finished"}),status:"success"}),(0,r.Z)(d,_.x8.Suspended,{text:(0,c.wv)({id:"page.instance.status.suspended"}),status:"warning"}),(0,r.Z)(d,_.x8.Faulted,{text:(0,c.wv)({id:"page.instance.status.faulted"}),status:"error"}),(0,r.Z)(d,_.x8.Cancelled,{text:(0,c.wv)({id:"page.instance.status.cancelled"}),status:"default"}),d)},90774:function(C,h,a){"use strict";a.d(h,{BK:function(){return D},mV:function(){return b},jP:function(){return x},qd:function(){return L},Cb:function(){return A},$5:function(){return P},aT:function(){return B},C4:function(){return o}});var r=a(39428),_=a(11849),c=a(3182),d=a(25377);function I(n,t){return f.apply(this,arguments)}function f(){return f=_asyncToGenerator(_regeneratorRuntime().mark(function n(t,s){return _regeneratorRuntime().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",request("/api/workflow-instances",_objectSpread({method:"DELETE",data:t,getResponse:!0},s||{})));case 1:case"end":return e.stop()}},n)})),f.apply(this,arguments)}function D(n,t){return v.apply(this,arguments)}function v(){return v=(0,c.Z)((0,r.Z)().mark(function n(t,s){return(0,r.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",(0,d.WY)("/api/workflow-instances/".concat(t),(0,_.Z)({method:"DELETE",getResponse:!0},s||{})));case 1:case"end":return e.stop()}},n)})),v.apply(this,arguments)}function b(n,t){return M.apply(this,arguments)}function M(){return M=(0,c.Z)((0,r.Z)().mark(function n(t,s){return(0,r.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",(0,d.WY)("/api/workflow-instances/".concat(t),(0,_.Z)({method:"GET"},s||{})));case 1:case"end":return e.stop()}},n)})),M.apply(this,arguments)}function x(n,t){return O.apply(this,arguments)}function O(){return O=(0,c.Z)((0,r.Z)().mark(function n(t,s){return(0,r.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",(0,d.WY)("/api/workflow-instances/".concat(t,"/execution-logs"),(0,_.Z)({method:"GET"},s||{})));case 1:case"end":return e.stop()}},n)})),O.apply(this,arguments)}function L(n,t){return T.apply(this,arguments)}function T(){return T=(0,c.Z)((0,r.Z)().mark(function n(t,s){return(0,r.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",(0,d.WY)("/api/workflow-instances",(0,_.Z)({method:"GET",params:t},s||{})));case 1:case"end":return e.stop()}},n)})),T.apply(this,arguments)}function A(n,t){return Z.apply(this,arguments)}function Z(){return Z=(0,c.Z)((0,r.Z)().mark(function n(t,s){return(0,r.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",(0,d.WY)("/api/workflow-instances/".concat(t,"/execution-logs/summary"),(0,_.Z)({method:"GET"},s||{})));case 1:case"end":return e.stop()}},n)})),Z.apply(this,arguments)}function P(n,t){return y.apply(this,arguments)}function y(){return y=(0,c.Z)((0,r.Z)().mark(function n(t,s){return(0,r.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",(0,d.WY)("/api/workflow-instances/statistics/count-date",(0,_.Z)({method:"GET",params:t},s||{})));case 1:case"end":return e.stop()}},n)})),y.apply(this,arguments)}function B(n,t){return R.apply(this,arguments)}function R(){return R=(0,c.Z)((0,r.Z)().mark(function n(t,s){return(0,r.Z)().wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",(0,d.WY)("/api/workflow-instances/".concat(t,"/cancel"),(0,_.Z)({method:"POST",getResponse:!0},s||{})));case 1:case"end":return e.stop()}},n)})),R.apply(this,arguments)}function u(n,t,s){return W.apply(this,arguments)}function W(){return W=_asyncToGenerator(_regeneratorRuntime().mark(function n(t,s,l){return _regeneratorRuntime().wrap(function(i){for(;;)switch(i.prev=i.next){case 0:return i.abrupt("return",request("/api/workflow-instances/".concat(t,"/dispatch"),_objectSpread({method:"POST",data:s,getResponse:!0},l||{})));case 1:case"end":return i.stop()}},n)})),W.apply(this,arguments)}function k(n,t,s){return g.apply(this,arguments)}function g(){return g=_asyncToGenerator(_regeneratorRuntime().mark(function n(t,s,l){return _regeneratorRuntime().wrap(function(i){for(;;)switch(i.prev=i.next){case 0:return i.abrupt("return",request("/api/workflow-instances/".concat(t,"/execute"),_objectSpread({method:"POST",data:s,getResponse:!0},l||{})));case 1:case"end":return i.stop()}},n)})),g.apply(this,arguments)}function o(n,t,s){return E.apply(this,arguments)}function E(){return E=(0,c.Z)((0,r.Z)().mark(function n(t,s,l){return(0,r.Z)().wrap(function(i){for(;;)switch(i.prev=i.next){case 0:return i.abrupt("return",(0,d.WY)("/api/workflow-instances/".concat(t,"/retry"),(0,_.Z)({method:"POST",data:s,getResponse:!0},l||{})));case 1:case"end":return i.stop()}},n)})),E.apply(this,arguments)}}}]);