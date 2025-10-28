window.todoIdb = (() => {
  const DB_NAME = 'todoapp-db';
  const DB_VER = 2; // <-- bump when you change schema
  const STORES = { assignees: 'assignees', tasks: 'tasks' };

  function openDb() {
    return new Promise((resolve, reject) => {
      const req = indexedDB.open(DB_NAME, DB_VER);

      req.onupgradeneeded = () => {
        const db = req.result;
        console.log('indexedDb: onupgradeneeded, creating stores... (ver', DB_VER, ')');

        // Assignees store — use camelCase id
        if (!db.objectStoreNames.contains(STORES.assignees)) {
          const s = db.createObjectStore(STORES.assignees, { keyPath: 'id' });
          s.createIndex('name', 'name', { unique: false });
          s.createIndex('email', 'email', { unique: false });
        }

        // Tasks store — use camelCase id + indexes in camelCase
        if (!db.objectStoreNames.contains(STORES.tasks)) {
          const s = db.createObjectStore(STORES.tasks, { keyPath: 'id' });
          s.createIndex('assigneeId', 'assigneeId', { unique: false });
          s.createIndex('dueDate', 'dueDate', { unique: false });
          s.createIndex('status', 'status', { unique: false });
          s.createIndex('priority', 'priority', { unique: false });
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

  // more explicit upsert with logging
  const upsert = (store, entity) =>
    tx(store, 'readwrite', s => {
      console.log('indexedDb.upsert -> store:', store, 'entity:', entity);
      const req = s.put(entity);
      req.onsuccess = () => console.log('indexedDb.put success', req.result);
      req.onerror = () => console.error('indexedDb.put error', req.error);
      return req;
    });

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
      const idx = s.index('assigneeId');
      const req = idx.getAll(assigneeId);
      req.onsuccess = () => {
        const arr = req.result || [];
        arr.sort((a, b) => {
          const ad = a.dueDate ?? '';
          const bd = b.dueDate ?? '';
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
    add: upsert,
    get: getOne,
    list: listAll,
    delete: del,
    listTasksByAssignee
  };
})();
