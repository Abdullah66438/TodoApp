// indexedDB helper goes here
// Minimal IndexedDB helper for TodoApp (assignees + tasks).
// Stores:
// - assignees: keyPath Id (Guid string)
// - tasks:     keyPath Id (Guid string), indexes: AssigneeId, DueDate (ISO string)

window.todoIdb = (() => {
    const DB_NAME = 'todoapp-db';
    const DB_VER = 1;
    const STORES = { assignees: 'assignees', tasks: 'tasks' };
  
    function openDb() {
      return new Promise((resolve, reject) => {
        const req = indexedDB.open(DB_NAME, DB_VER);
        req.onupgradeneeded = () => {
          const db = req.result;
  
          if (!db.objectStoreNames.contains(STORES.assignees)) {
            const s = db.createObjectStore(STORES.assignees, { keyPath: 'Id' });
            s.createIndex('Name', 'Name', { unique: false });
            s.createIndex('Email', 'Email', { unique: false });
          }
  
          if (!db.objectStoreNames.contains(STORES.tasks)) {
            const s = db.createObjectStore(STORES.tasks, { keyPath: 'Id' });
            s.createIndex('AssigneeId', 'AssigneeId', { unique: false });
            s.createIndex('DueDate', 'DueDate', { unique: false });
            s.createIndex('Status', 'Status', { unique: false });
            s.createIndex('Priority', 'Priority', { unique: false });
          }
        };
        req.onsuccess = () => resolve(req.result);
        req.onerror = () => reject(req.error);
      });
    }
  
    async function tx(storeName, mode, fn) {
      const db = await openDb();
      return new Promise((resolve, reject) => {
        const t = db.transaction(storeName, mode);
        const store = t.objectStore(storeName);
        const result = fn(store);
        t.oncomplete = () => resolve(result);
        t.onerror = () => reject(t.error);
      });
    }
  
    const upsert = (store, entity) =>
      tx(store, 'readwrite', s => s.put(entity));
  
    const getOne = (store, id) =>
      tx(store, 'readonly', s => new Promise((res, rej) => {
        const r = s.get(id);
        r.onsuccess = () => res(r.result ?? null);
        r.onerror = () => rej(r.error);
      }));
  
    const listAll = (store) =>
      tx(store, 'readonly', s => new Promise((res, rej) => {
        const r = s.getAll();
        r.onsuccess = () => res(r.result || []);
        r.onerror = () => rej(r.error);
      }));
  
    const del = (store, id) => tx(store, 'readwrite', s => s.delete(id));
  
    const listTasksByAssignee = (assigneeId) =>
      tx('tasks', 'readonly', s => new Promise((res, rej) => {
        const idx = s.index('AssigneeId');
        const req = idx.getAll(assigneeId);
        req.onsuccess = () => {
          const arr = req.result || [];
          // Sort by DueDate (ISO string) — nulls last
          arr.sort((a, b) => {
            const ad = a.DueDate ?? '';
            const bd = b.DueDate ?? '';
            if (!ad && !bd) return 0;
            if (!ad) return 1;
            if (!bd) return -1;
            return ad.localeCompare(bd);
          });
          res(arr);
        };
        req.onerror = () => rej(req.error);
      }));
  
    return {
      add: upsert, // upsert
      get: getOne,
      list: listAll,
      delete: del,
      listTasksByAssignee
    };
  })();
  